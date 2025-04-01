using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficInput
	{
		public Dictionary<WorldDirection, int> CarPerDirection { get; set; }
		public Dictionary<WorldDirection, int> AverageWaitingTimePerDirection { get; set; }
		public TimeSpan CurrentPhaseDuration { get; set; }

		public IEnumerable<float> ToAiInput()
		{
			List<float> result = new List<float>();

			List<WorldDirection> directions =
				[WorldDirection.North, WorldDirection.East, WorldDirection.South, WorldDirection.West];

			foreach (var direction in directions)
			{
				if (CarPerDirection.TryGetValue(direction, out int carsOnDirection))
				{
					result.Add(carsOnDirection);
				}
				else
				{
					result.Add(0);
				}

				if (AverageWaitingTimePerDirection.TryGetValue(direction, out int averageWaitingTimeOnDirection))
				{
					result.Add(averageWaitingTimeOnDirection);
				}
				else
				{
					result.Add(0);
				}
			}

			result.Add((float)CurrentPhaseDuration.TotalMilliseconds);

			return result.ToArray();
		}
	}
}
