using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class IntersectionCore : LocationEntity
	{
		public IntersectionCore(Intersection root, string name, int distance = 10) : base(root, name, distance)
		{
		}
	}
}
