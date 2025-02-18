using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Models.TrafficLights
{
	public class TrafficPhase
	{
		public TrafficPhase(string name)
		{
			Name = name;
		}

		public string Name { get; init; }
		public List<(InboundLane, LaneType)> LanesWithGreenLight { get; } = new();
	}
}
