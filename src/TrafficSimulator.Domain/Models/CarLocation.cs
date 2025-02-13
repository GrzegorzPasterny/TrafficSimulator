using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class CarLocation
	{
		public CarLocation(LocationEntity startLocation, int startDistance = 0)
		{
			Location = startLocation;
			CurrentDistance = startDistance;
		}

		public LocationEntity Location { get; set; }

		/// <summary>
		/// Distance on the <see cref="Location"/> it is currently in
		/// </summary>
		public int CurrentDistance { get; set; }

		/// <summary>
		/// Distance left to cover <see cref="Location"/>
		/// </summary>
		public int DistanceLeft => Location.Distance - CurrentDistance;
	}
}
