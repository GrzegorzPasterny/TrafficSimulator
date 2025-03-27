using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Lights.HandlerTypes
{
	public class ManualTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly ILogger<ManualTrafficLightsHandler> _logger;

		public ManualTrafficLightsHandler(TrafficPhasesHandler trafficPhasesHandler, ILogger<ManualTrafficLightsHandler> logger)
		{
			_trafficPhasesHandler = trafficPhasesHandler;
			_logger = logger;

			if (trafficPhasesHandler.TrafficPhases is not null)
			{
				_trafficPhasesHandler.SetPhase(_trafficPhasesHandler.TrafficPhases!.First(), TimeSpan.Zero);

				_logger.LogTrace("Traffic Lights initial phase set [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
			}
		}

		public TrafficPhase GetCurrentTrafficPhase()
		{
			return _trafficPhasesHandler.CurrentPhase!;
		}

		public void LoadIntersection(Intersection intersection)
		{
			_trafficPhasesHandler.LoadIntersection(intersection);

			_trafficPhasesHandler.SetPhase(_trafficPhasesHandler.TrafficPhases!.First(), TimeSpan.Zero);
			_logger.LogTrace("Traffic Lights initial phase set [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			return Task.FromResult(UnitResult.Success<Error>());
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			UnitResult<Error> setPhaseResult = _trafficPhasesHandler.SetPhase(trafficPhaseName, TimeSpan.Zero);

			if (setPhaseResult.IsFailure
				&& setPhaseResult.Error.Code == "TrafficSimulator.Application.TrafficLightsChangeAttemptedTooSoon")
			{
				return UnitResult.Success<Error>();
			}

			return setPhaseResult;
		}
	}
}
