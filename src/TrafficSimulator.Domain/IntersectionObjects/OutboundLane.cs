using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity, IEquatable<OutboundLane>
	{
		public OutboundLane(Intersection root, IntersectionObject? parent, WorldDirection worldDirection, string name = "", int distance = 100)
			: base(root, parent, distance, name)
		{
			WorldDirection = worldDirection;

			if (GetType() == typeof(OutboundLane) && string.IsNullOrEmpty(name))
			{
				DeterminePositionOfTheOutboundLane(root, worldDirection);
				Name = string.Concat(Name, "_", PositionOnTheLane);
			}
		}

		public WorldDirection WorldDirection { get; }

		/// <summary>
		/// Position of the lane on the direction from the left looking towards intersection core
		/// </summary>
		public int PositionOnTheLane { get; internal set; }

		private void DeterminePositionOfTheOutboundLane(Intersection root, WorldDirection worldDirection)
		{
			IEnumerable<OutboundLane> lanesOnTheSameDirection = root.ObjectLookup
				.OfType<OutboundLane>()
				.Where(lane => typeof(OutboundLane) == lane.GetType())
				.Where(lane => lane.WorldDirection == worldDirection);

			PositionOnTheLane = lanesOnTheSameDirection.Count();
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
