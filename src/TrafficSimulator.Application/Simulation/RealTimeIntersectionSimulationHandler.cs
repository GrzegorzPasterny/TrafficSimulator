using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Simulation
{
	public class RealTimeIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<RealTimeIntersectionSimulationHandler> _logger;
		private readonly TimeSpan _timeStep = TimeSpan.FromMilliseconds(100);
		private Stopwatch? _stopwatch;
		private System.Timers.Timer? _timer;
		private CancellationTokenSource? _cancellationTokenSource;

		public RealTimeIntersectionSimulationHandler(
			ISender sender,
			ITrafficLightsHandler trafficLightsHandler,
			ILogger<RealTimeIntersectionSimulationHandler> logger)
			: base(sender, trafficLightsHandler, logger)
		{
			_logger = logger;
		}

		public override void Dispose()
		{
			_cancellationTokenSource?.Dispose();
		}

		internal override Task SimulationRunner()
		{
			_timer = new System.Timers.Timer(_timeStep.TotalMilliseconds);
			_stopwatch = Stopwatch.StartNew();

			// Define a cancellation token for timeout
			_cancellationTokenSource = new CancellationTokenSource(IntersectionSimulation!.Options.Timeout);
			var cancellationToken = _cancellationTokenSource.Token;

			_timer.Elapsed += async (s, e) =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogWarning("Simulation aborted due to timeout [TimeLimit = {TimeLimit} ms]",
						IntersectionSimulation!.Options.Timeout);

					IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Aborted;
					_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));

					CleanupTimer();
					return;
				}

				await PerformSimulationStep();

				await DetermineState();

				if (IntersectionSimulation!.SimulationState.StepsCount >= IntersectionSimulation.Options.StepLimit)
				{
					IntersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
						"[StepLimit = {StepLimit}]", IntersectionSimulation.Options.StepLimit);

					_ = Task.Run(() => GatherResults(_stopwatch!.ElapsedMilliseconds));

					CleanupTimer();
					return;
				}

				if (IntersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
				{
					_logger.LogInformation("The simulation has finished");

					_ = Task.Run(() => GatherResults(_stopwatch!.ElapsedMilliseconds));

					CleanupTimer();
					return;
				}
			};

			_timer.Start();

			return Task.CompletedTask;
		}

		private void CleanupTimer()
		{
			_timer?.Stop();
			_timer?.Dispose();
			_timer = null;
		}
	}
}
