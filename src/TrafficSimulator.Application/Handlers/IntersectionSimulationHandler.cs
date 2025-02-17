using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers
{
	public class IntersectionSimulationHandler : ISimulationHandler
	{
		private IntersectionSimulation? _intersectionSimulation;
		private Stopwatch? _simulationTimer;
		private ICarGeneratorRepository _carGeneratorRepository;
		private readonly ICarRepository _carRepository;
		private readonly ILogger<IntersectionSimulationHandler> _logger;

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

		public async Task<ErrorOr<SimulationState>> GetState()
		{
			SimulationState simulationState = new SimulationState();
			// TODO return good Simulation State object, or Error
			simulationState.SimulationPhase = SimulationPhase;
			bool allCarsFinished;

			switch (SimulationPhase)
			{
				case SimulationPhase.NotStarted:
					break;
				case SimulationPhase.Finished:
					break;
				case SimulationPhase.InProgressCarGenerationFinished:
					allCarsFinished = await AllCarsFinished(simulationState);

					if (allCarsFinished)
					{
						simulationState.SimulationPhase = SimulationPhase.Finished;
					}
					break;
				case SimulationPhase.InProgress:
					bool allCarGeneratorsFinished = await AllCarGeneratorsFinished(simulationState);

					if (allCarGeneratorsFinished)
					{
						simulationState.SimulationPhase = SimulationPhase.InProgressCarGenerationFinished;
					}

					allCarsFinished = await AllCarsFinished(simulationState);

					if (allCarGeneratorsFinished && allCarsFinished)
					{
						simulationState.SimulationPhase = SimulationPhase.Finished;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException($"Invalid simulation state: {SimulationPhase}");
			}

			_logger.LogDebug("[Simulation state = {SimulationState}]", simulationState);

			return simulationState;
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

		private async Task<bool> AllCarGeneratorsFinished(SimulationState simulationState)
		{
			simulationState.CarGenerators.AddRange(await _carGeneratorRepository.GetCarGeneratorsAsync());

			return simulationState.CarGenerators.All(carGenerator => carGenerator.IsGenerationFinished().Value);
		}

		private async Task<bool> AllCarsFinished(SimulationState simulationState)
		{
			simulationState.Cars.AddRange(await _carRepository.GetCarsAsync());

			if (simulationState.Cars.Count == 0)
			{
				return false;
			}

			return simulationState.Cars.All(c => c.HasReachedDestination);
		}

		private async Task SimulationRunner()
		{
			using (CancellationTokenSource cts = new(_intersectionSimulation!.Options.Timeout))
			{
				while (cts.IsCancellationRequested is false)
				{
					// TODO: Move here some logic from GetState function

					// TODO: Invoke Move() method for car
				}
			}
		}
	}
}
