using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class IntersectionCore : LocationEntity
	{
		public IntersectionCore(Intersection root, IntersectionObject? parent, string name = "", int distance = 10)
			: base(root, parent, distance, name)
		{
		}
	}
}
