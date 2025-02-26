using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Intersection : IntersectionObject
	{
		public List<IntersectionObject> ObjectLookup { get; } = new();
		public List<Lanes> LanesCollection { get; set; } = new List<Lanes>();

		public IntersectionCore? IntersectionCore { get; set; }

		public List<TrafficPhase> TrafficPhases { get; } = [];

		public Intersection(string name, IntersectionObject? parent = null) : base(null!, parent, name)
		{
		}

		public void AddObject(IntersectionObject obj)
		{
			ObjectLookup.Add(obj);
		}

		public T? GetObject<T>(string fullName) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(o => o.FullName == fullName);
		}

		public T? GetObject<T>(Func<T, bool> predicate) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(predicate);
		}

		//public override string BuildObjectName()
		//{
		//	if (string.IsNullOrWhiteSpace(Parent?.Name))
		//	{
		//		return _name;
		//	}
		//	else
		//	{
		//		return $"{Parent?.Name}.{_name}";
		//	}
		//}
	}
}
