using CSharpFunctionalExtensions;
using ErrorOr;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers
{
	public class IntersectionSimulationHandler : ISimulationHandler
	{
		private IntersectionSimulation? _intersectionSimulation;
		private Stopwatch? _simulationTimer;
		private ICarGeneratorRepository _carGeneratorRepository;
		private readonly ICarRepository _carRepository;

		public SimulationPhase SimulationPhase { get; private set; }

		public IntersectionSimulationHandler(ICarGeneratorRepository carGeneratorRepository, ICarRepository carRepository)
		{
			_carGeneratorRepository = carGeneratorRepository;
			_carRepository = carRepository;
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

			switch (SimulationPhase)
			{
				case SimulationPhase.NotStarted:
					break;
				case SimulationPhase.Finished:
					break;
				case SimulationPhase.InProgress:
					IEnumerable<Car> cars = await _carRepository.GetCarsAsync();
					simulationState.Cars = cars.ToList();

					if (cars.All(c => c.HasReachedDestination))
					{
						SimulationPhase = SimulationPhase.Finished;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException($"Invalid simulation state: {SimulationPhase}");
			}

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
			return UnitResult.Success<Error>();
		}
	}
}
