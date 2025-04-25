using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficOutput
	{
		private readonly List<TrafficPhase> _phaseOrder;
		internal Dictionary<TrafficPhase, float> TrafficPhasePredictions { get; }

		public TrafficPhase BestTrafficPhase => TrafficPhasePredictions.MaxBy(x => x.Value).Key;

		public SimpleAiTrafficOutput(IEnumerable<TrafficPhase> possibleTrafficPhases)
		{
			_phaseOrder = possibleTrafficPhases.ToList();
			TrafficPhasePredictions = new Dictionary<TrafficPhase, float>();

			foreach (var phase in _phaseOrder)
			{
				TrafficPhasePredictions[phase] = 0f;
			}
		}

		internal void ApplyAiOutput(IReadOnlyList<float> outputs)
		{
			if (_phaseOrder.Count != outputs.Count)
			{
				throw new ArgumentException("Not able to interpret ML output.");
			}

			for (int i = 0; i < _phaseOrder.Count; i++)
			{
				TrafficPhasePredictions[_phaseOrder[i]] = outputs[i];
			}
		}

		internal void ApplyAiOutput(Span<double> outputs)
		{
			if (_phaseOrder.Count != outputs.Length)
			{
				throw new ArgumentException("Not able to interpret ML output.");
			}

			for (int i = 0; i < _phaseOrder.Count; i++)
			{
				TrafficPhasePredictions[_phaseOrder[i]] = (float)outputs[i];
			}
		}
	}

}
