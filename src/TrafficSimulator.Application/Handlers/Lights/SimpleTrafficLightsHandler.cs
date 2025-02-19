using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers.Lights
{
	/// <summary>
	/// Changes Traffic Lights phases one by one in some time interval
	/// </summary>
	public class SimpleTrafficLightsHandler
	{
		private readonly Intersection _intersection;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;

		// TODO: Add circular buffer here

		public TimeSpan CurrentPhaseTime { get; set; }

		// Options
		public TimeSpan TimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleTrafficLightsHandler(Intersection intersection, TrafficPhasesHandler trafficPhasesHandler)
		{
			_intersection = intersection;
			_trafficPhasesHandler = trafficPhasesHandler;
		}

		public async Task SetLights(TimeSpan timeElapsed)
		{
			CurrentPhaseTime = CurrentPhaseTime.Add(timeElapsed);

			if (CurrentPhaseTime >= TimeForOnePhase)
			{
				_trafficPhasesHandler.
			}
		}
	}
}
