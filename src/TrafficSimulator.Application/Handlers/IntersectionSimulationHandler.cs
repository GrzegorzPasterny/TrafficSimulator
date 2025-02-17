using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers
{
	public abstract class IntersectionSimulationHandler : ISimulationHandler
	{
		internal IntersectionSimulation? _intersectionSimulation;
		internal Stopwatch? _simulationTimer;
		internal ICarGeneratorRepository _carGeneratorRepository;
		internal readonly ICarRepository _carRepository;
		internal readonly ILogger<IntersectionSimulationHandler> _logger;
		internal SimulationState _simulationState = new SimulationState();

		public SimulationPhase SimulationPhase { get; private set; }

		public IntersectionSimulationHandler(
			ICarGeneratorRepository carGeneratorRepository, ICarRepository carRepository, ILogger<IntersectionSimulationHandler> logger)
		{
			_carGeneratorRepository = carGeneratorRepository;
			_carRepository = carRepository;
			_logger = logger;
		}

		public UnitResult<Error> Abort()
		{
			if (SimulationPhase != SimulationPhase.InProgress)
			{
				return DomainErrors.SimulationStateChange(SimulationPhase, SimulationPhase.Finished);
			}

			_simulationTimer?.Stop();
			return UnitResult.Success<Error>();
		}

		public SimulationState GetState()
		{
			return _simulationState;
		}

		public UnitResult<Error> LoadIntersection(Intersection intersection)
		{
			_intersectionSimulation = new(intersection);

			return UnitResult.Success<Error>();
		}

		/// <summary>
		/// Starts simulation
		/// </summary>
		/// <remarks>
		///		Starts Car generators.
		///		Initialize Traffic Lights controller
		/// </remarks>
		public UnitResult<Error> Start()
		{
			if (SimulationPhase != SimulationPhase.NotStarted)
			{
				return DomainErrors.SimulationStateChange(SimulationPhase, SimulationPhase.InProgress);
			}

			_simulationTimer = Stopwatch.StartNew();
			IEnumerable<ICarGenerator> carGenerators = _carGeneratorRepository.GetCarGeneratorsAsync().Result;

			foreach (var carGenerator in carGenerators)
			{
				// TODO: Handle result
				_ = carGenerator.StartGenerating();
			}

			SimulationPhase = SimulationPhase.InProgress;

			Task.Run(SimulationRunner);

			return UnitResult.Success<Error>();
		}

		internal async Task<bool> AllCarGeneratorsFinished(SimulationState simulationState)
		{
			simulationState.CarGenerators.AddRange(await _carGeneratorRepository.GetCarGeneratorsAsync());

			return simulationState.CarGenerators.All(carGenerator => carGenerator.IsGenerationFinished().Value);
		}

		internal async Task<bool> AllCarsFinished(SimulationState simulationState)
		{
			simulationState.Cars.AddRange(await _carRepository.GetCarsAsync());

			if (simulationState.Cars.Count == 0)
			{
				return false;
			}

			return simulationState.Cars.All(c => c.HasReachedDestination);
		}

		internal async Task DetermineState()
		{
			bool allCarsFinished;

			if (_simulationState.SimulationPhase == SimulationPhase.InProgressCarGenerationFinished)
			{
				allCarsFinished = await AllCarsFinished(_simulationState);

				if (allCarsFinished)
				{
					_simulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}
			else
			{
				bool allCarGeneratorsFinished = await AllCarGeneratorsFinished(_simulationState);

				if (allCarGeneratorsFinished)
				{
					_simulationState.SimulationPhase = SimulationPhase.InProgressCarGenerationFinished;
				}

				allCarsFinished = await AllCarsFinished(_simulationState);

				if (allCarGeneratorsFinished && allCarsFinished)
				{
					_simulationState.SimulationPhase = SimulationPhase.Finished;
				}
			}

			_logger.LogDebug("[Simulation state = {SimulationState}]", _simulationState);
		}

		internal async Task PerformSimulationStep()
		{
			IEnumerable<Car> cars = await _carRepository.GetCarsAsync();
			foreach (var car in cars)
			{
				car.Move(_intersectionSimulation!.Options.Step);
			}

			// TODO: Add car collision check
			// TODO: Cars need to wait in the queue when car is in front of them and also when Traffic light is orange, or red
		}

		internal abstract Task SimulationRunner();
	}
}
