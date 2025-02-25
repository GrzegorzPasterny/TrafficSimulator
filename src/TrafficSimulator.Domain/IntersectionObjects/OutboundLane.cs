using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity
	{
		public WorldDirection WorldDirection { get; }

		public OutboundLane(Intersection root, IntersectionObject? parent, WorldDirection worldDirection, int distance = 10)
			: base(root, parent, distance)
		{
			WorldDirection = worldDirection;
		}

		public override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(OutboundLane)}";
		}
	}
}
