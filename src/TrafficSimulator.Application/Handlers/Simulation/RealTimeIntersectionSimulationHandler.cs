using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Handlers.Simulation
{
	public class RealTimeIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<RealTimeIntersectionSimulationHandler> _logger;
		private readonly TimeSpan _timeStep = TimeSpan.FromMilliseconds(100);
		private Stopwatch? _stopwatch;
		private System.Timers.Timer? _timer;

		public RealTimeIntersectionSimulationHandler(ICarGeneratorRepository carGeneratorRepository, ICarRepository carRepository, ILogger<RealTimeIntersectionSimulationHandler> logger) : base(carGeneratorRepository, carRepository, logger)
		{
			_logger = logger;
		}

		internal override Task SimulationRunner()
		{
			_timer = new System.Timers.Timer(_timeStep.TotalMilliseconds);
			_timer.Elapsed += async (s, e) =>
			{
				// Move cars
				await PerformSimulationStep();

				await DetermineState();

				if (_intersectionSimulation!.SimulationState.StepsCount >= _intersectionSimulation.Options.StepLimit)
				{
					_intersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
						"[StepLimit = {StepLimit}]", _intersectionSimulation.Options.StepLimit);

					_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));

					_timer?.Stop();
					_timer?.Dispose();
					_timer = null;
					return;
				}

				if (_intersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
				{
					_logger.LogInformation("The simulation has finished");

					_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));

					_timer?.Stop();
					_timer?.Dispose();
					_timer = null;
					return;
				}
			};
			_timer.Start();

			_stopwatch = Stopwatch.StartNew();

			return Task.CompletedTask;
		}
	}
}
