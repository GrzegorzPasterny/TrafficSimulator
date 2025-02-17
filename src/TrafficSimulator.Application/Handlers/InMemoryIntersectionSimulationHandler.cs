using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Handlers
{
	public class InMemoryIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		public InMemoryIntersectionSimulationHandler(
			ICarGeneratorRepository carGeneratorRepository,
			ICarRepository carRepository,
			ILogger<IntersectionSimulationHandler> logger)
			: base(carGeneratorRepository, carRepository, logger)
		{
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
				}
			}
		}
	}
}
