using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class Lanes : IntersectionObject
	{
		public Lanes(Intersection root, string name, WorldDirection worldDirection) : base(root, name)
		{
			WorldDirection = worldDirection;
		}

		/// <summary>
		/// Collection of lanes from left to right that approach the intersection
		/// </summary>
		public List<Lane>? InboundLanes { get; set; } = null;

		/// <summary>
		/// Collection of lanes from left to right that exits the intersection
		/// </summary>
		public List<Lane>? OutboundLanes { get; set; } = null;

		public TrafficLights? TrafficLights { get; set; } = null;

		public WorldDirection WorldDirection { get; }

		/// <summary>
		/// Distance from the edge of the map to the <see cref="TrafficLights"/> that car needs to cover
		/// </summary>
		public int Distance { get; set; }
		// TODO: Add zebra crossings in further versions
	}
}
