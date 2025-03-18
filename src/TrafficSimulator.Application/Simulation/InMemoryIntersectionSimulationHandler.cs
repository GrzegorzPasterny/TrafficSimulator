using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Application.Lights.HandlerTypes;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Simulation
{
	public class InMemoryIntersectionSimulationHandler : IntersectionSimulationHandler
	{
		private new readonly ILogger<InMemoryIntersectionSimulationHandler> _logger;
		private Stopwatch? _stopwatch;

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
			// Nothing to dispose
		}

		internal override async Task SimulationRunner()
		{
			_stopwatch = Stopwatch.StartNew();

			using (CancellationTokenSource cts = new(IntersectionSimulation!.Options.Timeout))
			{
				while (cts.IsCancellationRequested is false)
				{
					// Move cars
					await PerformSimulationStep();

					await DetermineState();

					NotifyAboutSimulationState();

					if (IntersectionSimulation.SimulationState.StepsCount >= IntersectionSimulation.Options.StepLimit)
					{
						IntersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

						_logger.LogInformation("Simulation aborted due to reaching the maximum step amount " +
							"[StepLimit = {StepLimit}]", IntersectionSimulation.Options.StepLimit);

						await GatherResults(_stopwatch.ElapsedMilliseconds);
						return;
					}

					if (IntersectionSimulation.SimulationState.SimulationPhase is SimulationPhase.Finished)
					{
						_logger.LogDebug("The simulation has finished");

						await GatherResults(_stopwatch.ElapsedMilliseconds);
						return;
					}

					// small delay to allow other Tasks to run and to prevent CPU burning
					//await Task.Delay(16);
				}

				if (cts.IsCancellationRequested)
				{
					_logger.LogInformation("The simulation has reached timeout [Timeout = {Timeout}]", IntersectionSimulation.Options.Timeout);
					IntersectionSimulation.SimulationState.SimulationPhase = SimulationPhase.Aborted;

					await GatherResults(_stopwatch.ElapsedMilliseconds);
				}
			}
		}
	}
}
