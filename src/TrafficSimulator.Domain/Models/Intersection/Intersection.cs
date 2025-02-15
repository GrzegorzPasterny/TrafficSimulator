using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.Intersection
{
	public class Intersection : IntersectionObject
	{
		public Dictionary<string, IntersectionObject> ObjectLookup { get; } = new();
		public List<Lanes> Lanes { get; set; } = new List<Lanes>();

		public IntersectionCore IntersectionCore { get; set; }

		public Intersection(string name) : base(null!, name)
		{
			IntersectionCore = new(this, "intersection.core");
		}

		public void AddObject(IntersectionObject obj)
		{
			ObjectLookup[obj.Name] = obj;
		}

		public T? GetObject<T>(string name) where T : IntersectionObject
		{
			return ObjectLookup.TryGetValue(name, out var obj) ? obj as T : null;
		}
	}
}
