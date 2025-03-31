using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Cars.DeleteCars;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Application.Cars.MoveCar;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Lights.HandlerTypes;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.SimulationSetup.LoadSimulation;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Handlers.Simulation
{
	public abstract class IntersectionSimulationHandler : ISimulationHandler
	{
		public IntersectionSimulation? IntersectionSimulation { get; internal set; }
		private readonly ISender _sender;
		private readonly ITrafficLightsHandlerFactory _trafficLightsHandlerFactory;
		internal ITrafficLightsHandler? _trafficLightsHandler;
		internal readonly ILogger<IntersectionSimulationHandler> _logger;

		public event EventHandler<SimulationStateEventArgs>? SimulationUpdated;

		public SimulationState? SimulationState => IntersectionSimulation?.SimulationState;
		public SimulationResults? SimulationResults => IntersectionSimulation?.SimulationResults!;

		public IntersectionSimulationHandler(
			ISender sender,
			ITrafficLightsHandlerFactory trafficLightsHandlerFactory,
			ILogger<IntersectionSimulationHandler> logger)
		{
			_sender = sender;
			_trafficLightsHandlerFactory = trafficLightsHandlerFactory;
			_logger = logger;
		}

		public UnitResult<Error> LoadIntersection(IntersectionSimulation intersectionSimulation)
		{
			IntersectionSimulation = intersectionSimulation;

			if (intersectionSimulation.Options.TrafficLightHandlerType is not null)
			{
				_trafficLightsHandler =
					_trafficLightsHandlerFactory.CreateHandler(intersectionSimulation.Options.TrafficLightHandlerType);
			}
			else
			{
				// Create the default one
				_trafficLightsHandler =
					_trafficLightsHandlerFactory.CreateHandler();
			}

			_trafficLightsHandler.LoadIntersection(intersectionSimulation.Intersection);

			SimulationState.CarGenerators =
							IntersectionSimulation.Intersection.ObjectLookup.OfType<ICarGenerator>().ToList();

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
			if (IntersectionSimulation is null)
			{
				return ApplicationErrors.IntersectionUninitialized();
			}

			if (IntersectionSimulation!.SimulationState.SimulationPhase != SimulationPhase.NotStarted)
			{
				return DomainErrors.SimulationStateChange(IntersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.InProgress);
			}

			await _sender.Send(new DeleteCarsCommand());

			IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.InProgress;

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

		internal void NotifyAboutSimulationState()
		{
			SimulationStateEventArgs simulationStateEventArgs = new(SimulationState.StepsCount);
			AddTrafficLightsStatus(simulationStateEventArgs);
			AddCarsStatus(simulationStateEventArgs);

			SimulationUpdated?.Invoke(this, simulationStateEventArgs);
		}

		private void AddCarsStatus(SimulationStateEventArgs simulationStateEventArgs)
		{
			IEnumerable<Car> cars = _sender.Send(new GetCarsCommand()).GetAwaiter().GetResult();

			foreach (Car car in cars.Where(car => car.HasReachedDestination == false || car.HasJustReachedDestination))
			{
				simulationStateEventArgs.CarLocations.Add(car.Id, car.CurrentLocation);
			}
		}

		private void AddTrafficLightsStatus(SimulationStateEventArgs simulationStateEventArgs)
		{
			simulationStateEventArgs.CurrentTrafficPhaseName = _trafficLightsHandler.GetCurrentTrafficPhase().Name;

			IEnumerable<TrafficLight> trafficLightsCollection = IntersectionSimulation!.Intersection.ObjectLookup.OfType<TrafficLight>();

			foreach (TrafficLight trafficLights in trafficLightsCollection)
			{
				simulationStateEventArgs.TrafficLightsState.Add(trafficLights.Id, trafficLights.TrafficLightState);
			}
		}

		internal bool AllCarGeneratorsFinished()
		{
			return SimulationState.CarGenerators.All(carGenerator => carGenerator.IsGenerationCompleted);
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

			if (IntersectionSimulation!.SimulationState.SimulationPhase is SimulationPhase.InProgressCarGenerationFinished)
			{
				allCarsFinished = await AllCarsFinished(IntersectionSimulation!.SimulationState);

				if (allCarsFinished)
				{
					IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}
			else
			{
				bool allCarGeneratorsFinished = AllCarGeneratorsFinished();

				if (allCarGeneratorsFinished)
				{
					IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.InProgressCarGenerationFinished;
				}

				allCarsFinished = await AllCarsFinished(IntersectionSimulation!.SimulationState);

				if (allCarGeneratorsFinished && allCarsFinished)
				{
					IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}

			_logger.LogTrace("[SimulationState = {SimulationState}]", IntersectionSimulation!.SimulationState);
		}

		internal async Task PerformSimulationStep()
		{
			await GenerateNewCars();

			await SetTrafficLights();

			await MoveCars();

			// update metrics
			IntersectionSimulation!.SimulationState.StepsCount++;
			IntersectionSimulation!.SimulationState.ElapsedTime = IntersectionSimulation.Options.StepTimespan * IntersectionSimulation!.SimulationState.StepsCount;

			// TODO: Add car collision check
			// TODO: Cars need to wait in the queue when car is in front of them and also when Traffic light is orange, or red
		}

		private async Task GenerateNewCars()
		{
			foreach (ICarGenerator carGenerator in SimulationState.CarGenerators)
			{
				if (carGenerator.IsGenerationCompleted == false)
				{
					await carGenerator.Generate(IntersectionSimulation!.Options.StepTimespan);
				}
			}
		}

		private Task SetTrafficLights()
		{
			return _trafficLightsHandler.SetLights(IntersectionSimulation!.Options.StepTimespan);
		}

		private Task MoveCars()
		{
			return _sender.Send(new MoveAllCarsCommand(IntersectionSimulation!.Options.StepTimespan));
		}

		internal async Task GatherResults(long elapsedMilliseconds)
		{
			if (IntersectionSimulation!.SimulationResults is null)
			{
				IntersectionSimulation.SimulationResults = new SimulationResults();
			}

			IntersectionSimulation!.SimulationResults.TotalCalculationTimeMs = elapsedMilliseconds;
			IntersectionSimulation!.SimulationResults.SimulationStepsTaken = IntersectionSimulation.SimulationState.StepsCount;

			List<Car> cars = (await _sender.Send(new GetCarsCommand())).ToList();

			IntersectionSimulation.SimulationResults.CarsPassed = cars.Count();
			IntersectionSimulation.SimulationResults.TotalCarsIdleTimeMs =
				cars.Sum(c => c.MovesWhenCarWaited) * IntersectionSimulation.Options.StepTimespan.TotalMilliseconds;

			_logger.LogInformation("SimulationResults = {SimulationResults}", SimulationResults);

			NotifyAboutSimulationState();
		}

		public abstract UnitResult<Error> ChangeTrafficPhase(string trafficPhaseName);
		internal abstract Task SimulationRunner();
		public abstract void Dispose();
		public abstract UnitResult<Error> Abort();
	}
}
