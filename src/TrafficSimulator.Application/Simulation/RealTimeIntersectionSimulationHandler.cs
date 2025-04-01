using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Simulation
{
    public class RealTimeIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<RealTimeIntersectionSimulationHandler> _logger;
		private Stopwatch? _stopwatch;
		private System.Timers.Timer? _timer;
		private CancellationTokenSource? _cancellationTokenSource;

		public RealTimeIntersectionSimulationHandler(
			ISender sender,
			ITrafficLightsHandlerFactory trafficLightsHandlerFactory,
			ILogger<RealTimeIntersectionSimulationHandler> logger)
			: base(sender, trafficLightsHandlerFactory, logger)
		{
			_logger = logger;
		}

		public override UnitResult<Error> ChangeTrafficPhase(string trafficPhaseName)
		{
			return _trafficLightsHandler.SetLightsManually(trafficPhaseName);
		}

		public override void Dispose()
		{
			_cancellationTokenSource?.Dispose();
		}

		public override UnitResult<Error> Abort()
		{
			if (IntersectionSimulation is null)
			{
				_logger.LogWarning("Attempt to abort uninitialized simulation");
				// TODO: Better to return error for the user
				return UnitResult.Success<Error>();
			}

			if (IntersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Aborted)
			{
				_logger.LogWarning("Attempt to abort already aborted simulation");
				// TODO: Better to return error for the user
				return UnitResult.Success<Error>();
			}

			if (IntersectionSimulation!.SimulationState.SimulationPhase is SimulationPhase.Finished)
			{
				return DomainErrors.SimulationStateChange(IntersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.Finished);
			}

			_cancellationTokenSource?.Cancel();

			return UnitResult.Success<Error>();
		}

		internal override Task SimulationRunner()
		{
			_timer = new System.Timers.Timer(IntersectionSimulation!.Options.StepTimespan);
			_stopwatch = Stopwatch.StartNew();

			// Define a cancellation token for timeout
			_cancellationTokenSource = new CancellationTokenSource(IntersectionSimulation!.Options.Timeout);
			var cancellationToken = _cancellationTokenSource.Token;

			_timer.Elapsed += async (s, e) =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogWarning("The simulation was aborted, or reached timeout [Timeout = {Timeout}]",
						IntersectionSimulation!.Options.Timeout);

					IntersectionSimulation!.SimulationState.SimulationPhase = SimulationPhase.Aborted;
					_ = Task.Run(() => GatherResults(_stopwatch.ElapsedMilliseconds));

					CleanupTimer();
					return;
				}

				await PerformSimulationStep();

				await DetermineState();

				NotifyAboutSimulationState();

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
