using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficOutput
	{
		internal Dictionary<TrafficPhase, float> TrafficPhasePredictions { get; }
		public TrafficPhase BestTrafficPhase => TrafficPhasePredictions.MaxBy(x => x.Value).Key;

		public SimpleAiTrafficOutput(IEnumerable<TrafficPhase> possibleTrafficPhases)
		{
			TrafficPhasePredictions = new Dictionary<TrafficPhase, float>();

			foreach (var possiblePhase in possibleTrafficPhases)
			{
				TrafficPhasePredictions.Add(possiblePhase, 0);
			}
		}

		internal void ApplyAiOutput(IReadOnlyList<float> output)
		{
			if (TrafficPhasePredictions.Count != output.Count)
			{
				throw new ArgumentException("Not able to interpret ML output.");
			}

			for (var i = 0; i < TrafficPhasePredictions.Count; i++)
			{
				TrafficPhasePredictions[TrafficPhasePredictions.ElementAt(i).Key] = output[i];
			}
		}
	}
}
