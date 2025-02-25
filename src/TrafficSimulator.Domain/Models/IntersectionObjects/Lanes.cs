using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Lanes : IntersectionObject
	{
		public Lanes(Intersection root, IntersectionObject? parent, WorldDirection worldDirection) : base(root, parent)
		{
			WorldDirection = worldDirection;
			Name = BuildObjectName(parent!.Name);
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

		public override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(Lanes)}.{WorldDirection}";
		}
		// TODO: Add zebra crossings in further versions
	}
}
