using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Application.Cars.MoveCar;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.SimulationSetup.LoadSimulation;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Handlers.Simulation
{
	public abstract class IntersectionSimulationHandler : ISimulationHandler
	{
		internal IntersectionSimulation? _intersectionSimulation;
		private readonly ISender _sender;
		private readonly ITrafficLightsHandler _trafficLightsHandler;
		internal readonly ILogger<IntersectionSimulationHandler> _logger;

		public SimulationState SimulationState => _intersectionSimulation!.SimulationState;
		public SimulationResults SimulationResults => _intersectionSimulation!.SimulationResults!;

		public IntersectionSimulationHandler(
			ISender sender,
			ITrafficLightsHandler trafficLightsHandler,
			ILogger<IntersectionSimulationHandler> logger)
		{
			_sender = sender;
			_trafficLightsHandler = trafficLightsHandler;
			_logger = logger;
		}

		public UnitResult<Error> Abort()
		{
			if (_intersectionSimulation!.SimulationState.SimulationPhase is SimulationPhase.Finished)
			{
				return DomainErrors.SimulationStateChange(_intersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.Finished);
			}

			// TODO: Cancel the simulation

			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> LoadIntersection(IntersectionSimulation intersectionSimulation)
		{
			_intersectionSimulation = intersectionSimulation;
			_trafficLightsHandler.LoadIntersection(intersectionSimulation.Intersection);
			SimulationState.CarGenerators =
							_intersectionSimulation.Intersection.ObjectLookup.OfType<ICarGenerator>().ToList();

			return UnitResult.Success<Error>();
		}

		public async Task<UnitResult<Error>> LoadIntersection(string identifier)
		{
			ErrorOr<IntersectionSimulation> intersectionSimulationResult = await _sender.Send(new LoadSimulationCommand(identifier));

			if (intersectionSimulationResult.IsError)
			{
				// TODO: Combine Errors
				return intersectionSimulationResult.FirstError;
			}

			LoadIntersection(intersectionSimulationResult.Value);

			return UnitResult.Success<Error>();
		}

		/// <summary>
		/// Starts simulation
		/// </summary>
		/// <remarks>
		///		Starts Car generators.
		///		Initialize Traffic Lights controller
		/// </remarks>
		public async Task<UnitResult<Error>> Start()
		{
			if (_intersectionSimulation!.SimulationState.SimulationPhase != SimulationPhase.NotStarted)
			{
				return DomainErrors.SimulationStateChange(_intersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.InProgress);
			}

			_intersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.InProgress;

			try
			{
				// What I thought, but it looks like it is all wrong:
				// Task for Simulation Runner can complete at the end of the simulation (InMemory),
				// or it can be completed just after simulation is fired (RealTime),
				// so it is pointless to rely on any information from it
				await SimulationRunner();

				return UnitResult.Success<Error>();
			}
			catch (Exception ex)
			{
				_logger.LogError("Unhandled Exception occured while running the simulation [Error = {Error}]", ex);
				return ApplicationErrors.UnhandledSimulationException(ex);
			}
		}

		internal bool AllCarGeneratorsFinished()
		{
			return SimulationState.CarGenerators.All(carGenerator => carGenerator.IsGenerationFinished);
		}

		internal async Task<bool> AllCarsFinished(SimulationState simulationState)
		{
			simulationState.Cars = (await _sender.Send(new GetCarsCommand())).ToList();

			if (simulationState.Cars.Count == 0)
			{
				return false;
			}

			return simulationState.Cars.All(c => c.HasReachedDestination);
		}

		internal async Task DetermineState()
		{
			bool allCarsFinished;

			if (_intersectionSimulation!.SimulationState.SimulationPhase is SimulationPhase.InProgressCarGenerationFinished)
			{
				allCarsFinished = await AllCarsFinished(_intersectionSimulation!.SimulationState);

				if (allCarsFinished)
				{
					_intersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}
			else
			{
				bool allCarGeneratorsFinished = AllCarGeneratorsFinished();

				if (allCarGeneratorsFinished)
				{
					_intersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.InProgressCarGenerationFinished;
				}

				allCarsFinished = await AllCarsFinished(_intersectionSimulation!.SimulationState);

				if (allCarGeneratorsFinished && allCarsFinished)
				{
					_intersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}

			_logger.LogTrace("[SimulationState = {SimulationState}]", _intersectionSimulation!.SimulationState);
		}

		internal async Task PerformSimulationStep()
		{
			await GenerateNewCars();

			await SetTrafficLights();

			await MoveCars();

			// update metrics
			_intersectionSimulation!.SimulationState.StepsCount++;
			_intersectionSimulation!.SimulationState.ElapsedTime = _intersectionSimulation.Options.StepTimespan * _intersectionSimulation!.SimulationState.StepsCount;

			// TODO: Add car collision check
			// TODO: Cars need to wait in the queue when car is in front of them and also when Traffic light is orange, or red
		}

		private async Task GenerateNewCars()
		{
			foreach (ICarGenerator carGenerator in SimulationState.CarGenerators)
			{
				if (carGenerator.IsGenerationFinished == false)
				{
					await carGenerator.Generate(_intersectionSimulation!.Options.StepTimespan);
				}
			}
		}

		private Task SetTrafficLights()
		{
			return _trafficLightsHandler.SetLights(_intersectionSimulation!.Options.StepTimespan);
		}

		private Task MoveCars()
		{
			return _sender.Send(new MoveAllCarsCommand(_intersectionSimulation!.Options.StepTimespan));
		}

		internal async Task GatherResults(long elapsedMilliseconds)
		{
			if (_intersectionSimulation!.SimulationResults is null)
			{
				_intersectionSimulation.SimulationResults = new SimulationResults();
			}

			_intersectionSimulation!.SimulationResults.TotalCalculationTimeMs = elapsedMilliseconds;
			_intersectionSimulation!.SimulationResults.SimulationStepsTaken = _intersectionSimulation.SimulationState.StepsCount;

			List<Car> cars = (await _sender.Send(new GetCarsCommand())).ToList();

			_intersectionSimulation.SimulationResults.CarsPassed = cars.Count();
			_intersectionSimulation.SimulationResults.TotalCarsIdleTimeMs =
				cars.Sum(c => c.MovesWhenCarWaited) * _intersectionSimulation.Options.StepTimespan.TotalMilliseconds;

			_logger.LogInformation("SimulationResults = {SimulationResults}", SimulationResults);
		}

		internal abstract Task SimulationRunner();
		public abstract void Dispose();

	}
}
