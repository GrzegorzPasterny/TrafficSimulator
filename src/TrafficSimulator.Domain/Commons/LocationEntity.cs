using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class LocationEntity : IntersectionObject
	{
		/// <summary>
		/// Distance the vehicle must cover to pass location
		/// </summary>
		public int Distance { get; set; }

		public LocationEntity(Intersection root, int distance) : base(root)
		{
			Distance = distance;
		}
	}
}
