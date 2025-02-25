using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Helpers;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.TrafficPhases;
using TrafficSimulator.Domain.IntersectionObjects.IntersectionProperties;

namespace TrafficSimulator.Application.Handlers.Lights
{
    /// <summary>
    /// Changes Traffic Lights phases one by one in some time interval
    /// </summary>
    public class SimpleSequentialTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly ILogger<SimpleSequentialTrafficLightsHandler> _logger;
		private readonly CircularList<TrafficPhase> _circularListForTrafficPhases;

		public TimeSpan CurrentPhaseTime { get; set; }

		// Options
		public TimeSpan TimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleSequentialTrafficLightsHandler(TrafficPhasesHandler trafficPhasesHandler, ILogger<SimpleSequentialTrafficLightsHandler> logger)
		{
			_trafficPhasesHandler = trafficPhasesHandler;
			_logger = logger;
			_circularListForTrafficPhases = new CircularList<TrafficPhase>(trafficPhasesHandler.TrafficPhases);
			_trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current);

			_logger.LogInformation("Traffic Lights initial phase set [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			CurrentPhaseTime = CurrentPhaseTime.Add(timeElapsed);

			if (CurrentPhaseTime >= TimeForOnePhase)
			{
				_circularListForTrafficPhases.MoveNext();

				// TODO: Handle
				_ = _trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current);

				CurrentPhaseTime = TimeSpan.Zero;

				_logger.LogInformation("Traffic Lights phase changed [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
			}

			return Task.FromResult(UnitResult.Success<Error>());
		}
	}
}
