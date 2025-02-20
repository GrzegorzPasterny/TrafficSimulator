using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Helpers;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	/// <summary>
	/// Changes Traffic Lights phases one by one in some time interval
	/// </summary>
	public class SimpleTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly CircularList<TrafficPhase> _circularListForTrafficPhases;

		public TimeSpan CurrentPhaseTime { get; set; }

		// Options
		public TimeSpan TimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleTrafficLightsHandler(TrafficPhasesHandler trafficPhasesHandler)
		{
			_trafficPhasesHandler = trafficPhasesHandler;

			_circularListForTrafficPhases = new CircularList<TrafficPhase>(trafficPhasesHandler.TrafficPhases);
			_trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current);
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			CurrentPhaseTime = CurrentPhaseTime.Add(timeElapsed);

			if (CurrentPhaseTime >= TimeForOnePhase)
			{
				_circularListForTrafficPhases.MoveNext();

				// TODO: Handle
				_ = _trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current);
			}

			return Task.FromResult(UnitResult.Success<Error>());
		}
	}
}
