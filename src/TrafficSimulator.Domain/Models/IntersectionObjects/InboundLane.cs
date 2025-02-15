using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane
	{
		public InboundLane(Intersection root, LaneType[] laneTypes, int distance = 10) : base(root, laneTypes, distance)
		{
		}

		public ICarGenerator CarGenerator { get; set; }

		internal override string BuildObjectName(string parentName)
		{
			return $".{nameof(InboundLane)}";
		}
	}
}
