using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Handlers
{
	public class RealTimeIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<RealTimeIntersectionSimulationHandler> _logger;
		private readonly TimeSpan _timeStep = TimeSpan.FromMilliseconds(100);
		private Stopwatch _stopwatch;

		public RealTimeIntersectionSimulationHandler(ICarGeneratorRepository carGeneratorRepository, ICarRepository carRepository, ILogger<RealTimeIntersectionSimulationHandler> logger) : base(carGeneratorRepository, carRepository, logger)
		{
			_logger = logger;
		}

		internal override async Task SimulationRunner()
		{
			var timer = new System.Timers.Timer(_timeStep.TotalMilliseconds);
			timer.Elapsed += async (s, e) =>
			{
				// Move cars
				await PerformSimulationStep();

				await DetermineState();

				if (_intersectionSimulation!.SimulationState.StepsCount >= _intersectionSimulation.Options.StepLimit)
				{
					_intersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					_stopwatch.Stop();
					_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
						"[StepLimit = {StepLimit}]", _intersectionSimulation.Options.StepLimit);

					_intersectionSimulation.SimulationResults.TotalCalculationTimeMs = _stopwatch.ElapsedMilliseconds;

					return;
				}

				if (_intersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
				{
					_stopwatch.Stop();
					_logger.LogInformation("The simulation has finished");
					return;
				}
			};
			timer.Start();

			_stopwatch = Stopwatch.StartNew();
		}
	}
}
