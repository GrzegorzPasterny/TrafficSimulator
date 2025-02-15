using TrafficSimulator.Domain.Models.Intersection;

namespace TrafficSimulator.Domain.Commons
{
	public class LocationEntity : IntersectionObject
	{
		/// <summary>
		/// Distance the vehicle must cover to pass location
		/// </summary>
		public int Distance { get; set; }

		public LocationEntity(Intersection root, string name, int distance) : base(root, name)
		{
			Distance = distance;
		}
	}
}
