using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Simulation
{
	public class InMemoryIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<InMemoryIntersectionSimulationHandler> _logger;
		private Stopwatch? _stopwatch;
		CancellationTokenSource? _cancellationTokenSource;

		public InMemoryIntersectionSimulationHandler(
			ISender sender,
			ITrafficLightsHandlerFactory trafficLightsHandlerFactory,
			ILogger<InMemoryIntersectionSimulationHandler> logger)
			: base(sender, trafficLightsHandlerFactory, logger)
		{
			_logger = logger;
		}

		public override UnitResult<Error> ChangeTrafficPhase(string trafficPhaseName)
		{
			return ApplicationErrors.TrafficPhaseManualChangeAttemptInInMemoryMode();
		}

		public override void Dispose()
		{
			_cancellationTokenSource?.Dispose();
		}

		public override UnitResult<Error> Abort()
		{
			if (IntersectionSimulation!.SimulationState.SimulationPhase is SimulationPhase.Finished)
			{
				return DomainErrors.SimulationStateChange(IntersectionSimulation!.SimulationState.SimulationPhase, SimulationPhase.Finished);
			}

			_cancellationTokenSource?.Cancel();

			return UnitResult.Success<Error>();
		}

		internal override async Task SimulationRunner()
		{
			_stopwatch = Stopwatch.StartNew();

			using (_cancellationTokenSource = new(IntersectionSimulation!.Options.Timeout))
			{
				while (_cancellationTokenSource.IsCancellationRequested is false)
				{
					// Move cars
					await PerformSimulationStep();

					await DetermineState();

					if (IntersectionSimulation.SimulationState.StepsCount >= IntersectionSimulation.Options.StepLimit)
					{
						IntersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

						_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
							"[StepLimit = {StepLimit}]", IntersectionSimulation.Options.StepLimit);

						NotifyAboutSimulationState();
						await GatherResults(_stopwatch.ElapsedMilliseconds);
						return;
					}

					if (IntersectionSimulation.SimulationState.SimulationPhase
							is SimulationPhase.Finished or SimulationPhase.Aborted)
					{
						_logger.LogDebug("The simulation has finished");

						NotifyAboutSimulationState();
						await GatherResults(_stopwatch.ElapsedMilliseconds);
						return;
					}
				}

				if (_cancellationTokenSource.IsCancellationRequested)
				{
					_logger.LogWarning("The simulation was aborted, or reached timeout [Timeout = {Timeout}]",
						IntersectionSimulation!.Options.Timeout);

					IntersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					NotifyAboutSimulationState();
					await GatherResults(_stopwatch.ElapsedMilliseconds);
				}
			}
		}
	}
}
