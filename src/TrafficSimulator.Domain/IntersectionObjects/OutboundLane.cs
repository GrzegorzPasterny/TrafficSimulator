using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity, IEquatable<OutboundLane>
	{
		public WorldDirection WorldDirection { get; }

		public OutboundLane(Intersection root, IntersectionObject? parent, WorldDirection worldDirection, string name = "", int distance = 100)
			: base(root, parent, distance, name)
		{
			WorldDirection = worldDirection;
		}

		public override bool Equals(object? obj)
		{
			return obj is OutboundLane other && Equals(other);
		}

		public bool Equals(OutboundLane? other)
		{
			if (other == null) return false;

			return base.Equals(other) && WorldDirection == other.WorldDirection;
		}
	}
}
