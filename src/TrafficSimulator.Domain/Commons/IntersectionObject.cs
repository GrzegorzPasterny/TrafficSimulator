using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class IntersectionObject : Entity
	{
		public IntersectionObject? Parent { get; private set; }
		public Intersection Root { get; private set; }
		public string Name { get; internal set; }

		protected IntersectionObject(Intersection root, IntersectionObject? parent)
		{
			// If root is null check if class itself is Intersection.
			// If yes reference itself to Root, otherwise throw to prevent misuse
			Root = root ?? (this as Intersection ?? throw new ArgumentNullException(nameof(root)));
			Parent = parent;

			Name = BuildObjectName(Parent is null ? "" : Parent.Name);
			Root.ObjectLookup.Add(this);
		}

		public abstract string BuildObjectName(string parentName);
	}
}
