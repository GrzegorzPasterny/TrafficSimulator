using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class IntersectionCore : LocationEntity, IEquatable<IntersectionCore>
	{
		public IntersectionCore(Intersection root, IntersectionObject? parent, string name = "", int distance = 10)
			: base(root, parent, distance, name)
		{
		}

		public override bool Equals(object? obj)
		{
			return obj is IntersectionCore other && Equals(other);
		}

		public bool Equals(IntersectionCore? other)
		{
			if (other == null) return false;

			// Compare based on essential properties, avoiding recursive relationships
			return FullName == other.FullName
				&& Distance == other.Distance;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, FullName, Distance);
		}
	}
}
