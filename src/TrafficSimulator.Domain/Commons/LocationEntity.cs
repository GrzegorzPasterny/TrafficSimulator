using CSharpFunctionalExtensions;

namespace TrafficSimulator.Domain.Commons
{
	public class LocationEntity : Entity
	{
		public int Distance { get; set; }

		public LocationEntity(int distance)
		{
			Distance = distance;
		}
	}
}
