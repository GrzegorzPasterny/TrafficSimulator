namespace TrafficSimulator.Domain.Models
{
	public class Lanes
	{
		/// <summary>
		/// Collection of lanes from left to right that approach the intersection
		/// </summary>
		public List<Lane>? IncommingLanes { get; set; } = null;

		/// <summary>
		/// Collection of lanes from left to right that exits the intersection
		/// </summary>
		public List<Lane>? OutcommingLanes { get; set; } = null;

		public TrafficLights? TrafficLights { get; set; } = null;

		/// <summary>
		/// Distance from the edge of the map to the <see cref="TrafficLights"/> that car needs to cover
		/// </summary>
		public int Distance { get; set; }
		// TODO: Add zebra crossings in further versions
	}
}
