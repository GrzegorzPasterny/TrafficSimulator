using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity
	{
		public WorldDirection WorldDirection { get; }

		public OutboundLane(Intersection root, IntersectionObject? parent, WorldDirection worldDirection, string name = "", int distance = 10)
			: base(root, parent, distance, name)
		{
			WorldDirection = worldDirection;
		}
	}
}
