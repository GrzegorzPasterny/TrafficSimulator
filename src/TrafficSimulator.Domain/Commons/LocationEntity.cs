using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class LocationEntity : IntersectionObject
	{
		/// <summary>
		/// Distance in meters the vehicle must cover to pass location
		/// </summary>
		public int Distance { get; set; }

		public LocationEntity(Intersection root, IntersectionObject? parent, int distance) : base(root, parent)
		{
			Distance = distance;
		}
	}
}
