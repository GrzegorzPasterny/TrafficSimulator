using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class IntersectionCore : LocationEntity
	{
		public IntersectionCore(int distance = 10) : base(distance)
		{
		}
	}
}
