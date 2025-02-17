using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane
	{
		public InboundLane(Intersection root, IntersectionObject? parent, LaneType[] laneTypes, int distance = 10)
			: base(root, parent, distance)
		{
			LaneTypes = laneTypes;
		}

		public ICarGenerator CarGenerator { get; set; }
		public LaneType[] LaneTypes { get; }

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(InboundLane)}";
		}
	}
}
