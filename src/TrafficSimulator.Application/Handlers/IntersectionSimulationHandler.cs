using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
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
		internal ICarGeneratorRepository _carGeneratorRepository;
		internal readonly ICarRepository _carRepository;
		internal readonly ILogger<IntersectionSimulationHandler> _logger;

		public SimulationState SimulationState => _intersectionSimulation!.SimulationState;
		public SimulationResults SimulationResults => _intersectionSimulation!.SimulationResults;

		public IntersectionSimulationHandler(
			ICarGeneratorRepository carGeneratorRepository, ICarRepository carRepository, ILogger<IntersectionSimulationHandler> logger)
		{
			_carGeneratorRepository = carGeneratorRepository;
			_carRepository = carRepository;
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
			if (_intersectionSimulation!.SimulationState.SimulationPhase != SimulationPhase.NotStarted)
			{
				return DomainErrors.SimulationStateChange(_intersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.InProgress);
			}

			IEnumerable<ICarGenerator> carGenerators = _carGeneratorRepository.GetCarGeneratorsAsync().Result;

			foreach (var carGenerator in carGenerators)
			{
				// TODO: Handle result
				_ = carGenerator.StartGenerating();
			}

			_intersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.InProgress;

			Task.Run(SimulationRunner);

			return UnitResult.Success<Error>();
		}

		internal async Task<bool> AllCarGeneratorsFinished(SimulationState simulationState)
		{
			simulationState.CarGenerators = (await _carGeneratorRepository.GetCarGeneratorsAsync()).ToList();

			return simulationState.CarGenerators.All(carGenerator => carGenerator.IsGenerationFinished().Value);
		}

		internal async Task<bool> AllCarsFinished(SimulationState simulationState)
		{
			simulationState.Cars = (await _carRepository.GetCarsAsync()).ToList();

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
				bool allCarGeneratorsFinished = await AllCarGeneratorsFinished(_intersectionSimulation!.SimulationState);

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
			IEnumerable<Car> cars = await _carRepository.GetCarsAsync();
			foreach (var car in cars)
			{
				car.Move(_intersectionSimulation!.Options.StepTimespan);
			}

			// update metrics
			_intersectionSimulation!.SimulationState.StepsCount++;
			_intersectionSimulation!.SimulationState.ElapsedTime = _intersectionSimulation.Options.StepTimespan * _intersectionSimulation!.SimulationState.StepsCount;

			// TODO: Add car collision check
			// TODO: Cars need to wait in the queue when car is in front of them and also when Traffic light is orange, or red
		}

		internal async Task GatherResults(long elapsedMilliseconds)
		{
			if (_intersectionSimulation!.SimulationResults is null)
			{
				_intersectionSimulation.SimulationResults = new SimulationResults();
			}

			_intersectionSimulation!.SimulationResults.TotalCalculationTimeMs = elapsedMilliseconds;
			// TODO: Fill in value
			_intersectionSimulation.SimulationResults.AverageCarIdleTime = 0;
			_intersectionSimulation.SimulationResults.CarsPassed = (await _carRepository.GetCarsAsync()).Count();
		}

		internal abstract Task SimulationRunner();
	}
}
