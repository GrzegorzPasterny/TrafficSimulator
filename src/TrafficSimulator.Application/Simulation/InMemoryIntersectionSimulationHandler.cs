using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Simulation
{
	public class InMemoryIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<InMemoryIntersectionSimulationHandler> _logger;
		private Stopwatch? _stopwatch;

		public InMemoryIntersectionSimulationHandler(
			ICarGeneratorRepository carGeneratorRepository,
			ICarRepository carRepository,
			ITrafficLightsHandler trafficLightsHandler,
			ISimulationSetupRepository simulationSetupRepository,
			ILogger<InMemoryIntersectionSimulationHandler> logger)
			: base(carGeneratorRepository, carRepository, trafficLightsHandler, simulationSetupRepository, logger)
		{
			_logger = logger;
		}

		public override void Dispose()
		{
			// Nothing to dispose
		}

		internal override async Task SimulationRunner()
		{
			_stopwatch = Stopwatch.StartNew();

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

						_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));
						return;
					}

					if (_intersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
					{
						_logger.LogInformation("The simulation has finished");

						_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));
						return;
					}

					// small delay to allow other Tasks to run and to prevent CPU burning
					//await Task.Delay(16);
				}

				if (cts.IsCancellationRequested)
				{
					_logger.LogInformation("The simulation has reached timeout [Timeout = {Timeout}]", _intersectionSimulation.Options.Timeout);
					_intersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));
				}
			}
		}
	}
}
