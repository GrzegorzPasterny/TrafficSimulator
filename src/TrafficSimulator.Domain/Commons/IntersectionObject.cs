using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Domain.Commons
{
	public abstract class IntersectionObject : Entity
	{
		public IntersectionObject? Parent { get; private set; }
		public Intersection Root { get; private set; }
		public string Name { get; private set; }

		protected IntersectionObject(Intersection root, string name, IntersectionObject? parent = null)
		{
			// If root is null check if class itself is Intersection.
			// If yes reference itself to Root, otherwise throw to prevent misuse
			Root = root ?? (this as Intersection ?? throw new ArgumentNullException(nameof(root)));

			Parent = parent;
			Name = name;
		}
	}
}
