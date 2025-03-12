using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class LocationEntity : IntersectionObject, IEquatable<LocationEntity>
	{
		/// <summary>
		/// Distance in meters the vehicle must cover to pass location
		/// </summary>
		public int Distance { get; set; }

		public LocationEntity(Intersection root, IntersectionObject? parent, int distance, string name = "") : base(root, parent, name)
		{
			Distance = distance;
		}

		public override bool Equals(object? obj)
		{
			return obj is LocationEntity other && Equals(other);
		}

		public bool Equals(LocationEntity? other)
		{
			if (other == null) return false;

			return base.Equals(other) && Distance == other.Distance;
		}
	}
}
