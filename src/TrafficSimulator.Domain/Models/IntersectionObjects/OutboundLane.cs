using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity
	{
		public OutboundLane(Intersection root, LaneType[] laneTypes, int distance = 10) : base(root, distance)
		{
			LaneType = laneTypes;
		}

		public LaneType[] LaneType { get; }


		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(OutboundLane)}";
		}
	}
}
