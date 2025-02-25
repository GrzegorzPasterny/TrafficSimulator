using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Lanes : IntersectionObject
	{
		public Lanes(Intersection root, IntersectionObject? parent, WorldDirection worldDirection, string name = "") : base(root, parent, name)
		{
			WorldDirection = worldDirection;
			Name = BuildDefaultObjectName();
		}

		/// <summary>
		/// Collection of lanes from left to right that approach the intersection
		/// </summary>
		public List<InboundLane>? InboundLanes { get; set; } = new();

		/// <summary>
		/// Collection of lanes from left to right that exits the intersection
		/// </summary>
		public List<OutboundLane>? OutboundLanes { get; set; } = new();

		public WorldDirection WorldDirection { get; }

		public override string BuildDefaultObjectName()
		{
			return $"{nameof(Lanes)}.{WorldDirection}";
		}
		// TODO: Add zebra crossings in further versions
	}
}
