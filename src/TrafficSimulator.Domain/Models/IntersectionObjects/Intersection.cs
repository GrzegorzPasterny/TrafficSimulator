using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Intersection : IntersectionObject
	{
		public List<IntersectionObject> ObjectLookup { get; } = new();
		public List<Lanes> LanesCollection { get; set; } = new List<Lanes>();

		public IntersectionCore? IntersectionCore { get; set; }

		public Intersection(string name, IntersectionObject? parent = null) : base(null!, parent)
		{
		}

		public void AddObject(IntersectionObject obj)
		{
			ObjectLookup.Add(obj);
		}

		public T? GetObject<T>(string name) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(o => o.Name == name);
		}

		public T? GetObject<T>(Func<T, bool> predicate) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(predicate);
		}

		internal override string BuildObjectName(string parentName)
		{
			if (string.IsNullOrWhiteSpace(parentName))
			{
				return nameof(Intersection);
			}
			else
			{
				return $"{parentName}.{nameof(Intersection)}";
			}

		}
	}
}
