using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class IntersectionObject : Entity<Guid>, IEquatable<IntersectionObject>
	{
		public IntersectionObject Parent { get; private set; }
		public Intersection Root { get; private set; }
		public string Name { get; internal set; }
		public string FullName => BuildObjectFullName();

		protected IntersectionObject(Intersection root, IntersectionObject? parent, string name = "")
		{
			// If root is null check if class itself is Intersection.
			// If yes reference itself to Root, otherwise throw to prevent misuse
			Id = Guid.NewGuid();
			Root = root ?? (this as Intersection ?? throw new ArgumentNullException(nameof(root)));
			Parent = parent;

			if (string.IsNullOrWhiteSpace(name))
			{
				Name = BuildDefaultObjectName();
			}
			else
			{
				Name = name;
			}

			Root.ObjectLookup.Add(this);
		}


		public virtual string BuildDefaultObjectName()
		{
			return GetType().Name;
		}

		public virtual string BuildObjectFullName()
		{
			return $"{Parent?.FullName}.{Name}";
		}

		public override bool Equals(object? obj)
		{
			return obj is IntersectionObject other && Equals(other);
		}

		public bool Equals(IntersectionObject? other)
		{
			if (other == null) return false;

			// TODO: Should ID be also used for comparison?
			return FullName == other.FullName;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
