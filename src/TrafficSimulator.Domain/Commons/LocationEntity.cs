using CSharpFunctionalExtensions;

namespace TrafficSimulator.Domain.Commons
{
	public class LocationEntity : Entity
	{
		/// <summary>
		/// Distance the vehicle must cover to pass location
		/// </summary>
		public int Distance { get; set; }

		public LocationEntity(int distance)
		{
			Distance = distance;
		}
	}
}
