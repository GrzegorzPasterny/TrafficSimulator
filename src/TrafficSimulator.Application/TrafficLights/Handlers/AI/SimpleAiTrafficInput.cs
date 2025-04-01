using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficInput
	{
		public Dictionary<WorldDirection, int> CarPerLane { get; set; }
		public Dictionary<WorldDirection, int> AverageWaitingTimePerLane { get; set; }
		public TimeSpan CurrentPhaseDuration { get; set; }

	}
}
