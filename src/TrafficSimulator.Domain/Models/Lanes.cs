namespace TrafficSimulator.Domain.Models
{
	public class Lanes
	{
		/// <summary>
		/// Collection of lanes from left to right
		/// </summary>
		public List<Lane>? LanesCollection { get; set; } = null;

		public TrafficLights? TrafficLights { get; set; } = null;

		/// <summary>
		/// Distance from the edge of the map to the <see cref="TrafficLights"/>.
		/// </summary>
		public int Distance { get; set; }
		// TODO: Add zebra crossings in further versions
	}
}
