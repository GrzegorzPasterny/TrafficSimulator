using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficOutput
	{
		public Dictionary<TrafficPhase, float> TrafficPhasePredictions { get; set; }
		public TrafficPhase BestTrafficPhase { get; set; }

		internal static SimpleAiTrafficOutput FromAiOutput(IReadOnlyList<float> output)
		{
			throw new NotImplementedException();
		}
	}
}
