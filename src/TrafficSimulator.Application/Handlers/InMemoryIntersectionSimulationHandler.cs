using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Handlers
{
	public class InMemoryIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private readonly ILogger<InMemoryIntersectionSimulationHandler> _logger;

		public InMemoryIntersectionSimulationHandler(
			ICarGeneratorRepository carGeneratorRepository,
			ICarRepository carRepository,
			ILogger<InMemoryIntersectionSimulationHandler> logger)
			: base(carGeneratorRepository, carRepository, logger)
		{
			_logger = logger;
		}

		internal override async Task SimulationRunner()
		{
			using (CancellationTokenSource cts = new(_intersectionSimulation!.Options.Timeout))
			{
				while (cts.IsCancellationRequested is false)
				{
					// Move cars
					await PerformSimulationStep();

					await DetermineState();

					if (_intersectionSimulation.SimulationState.StepsCount >= _intersectionSimulation.Options.StepLimit)
					{
						_intersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

						_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
							"[StepLimit = {StepLimit}]", _intersectionSimulation.Options.StepLimit);
						return;
					}

					if (_intersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
					{
						_logger.LogInformation("The simulation has finished");
						return;
					}
				}

				if (cts.IsCancellationRequested)
				{
					_logger.LogInformation("The simulation has reached timeout [Timeout = {Timeout}]", _intersectionSimulation.Options.Timeout);
					_intersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;
				}
			}
		}
	}
}
