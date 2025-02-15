using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Intersection : IntersectionObject
	{
		public Dictionary<string, IntersectionObject> ObjectLookup { get; } = new();
		public List<Lanes> LanesCollection { get; set; } = new List<Lanes>();

		public IntersectionCore? IntersectionCore { get; set; }

		public Intersection(string name) : base(null!)
		{
		}

		public void AddObject(IntersectionObject obj)
		{
			ObjectLookup[obj.Name] = obj;
		}

		public T? GetObject<T>(string name) where T : IntersectionObject
		{
			return ObjectLookup.TryGetValue(name, out var obj) ? obj as T : null;
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
