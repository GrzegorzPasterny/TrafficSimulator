using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class IntersectionCore : LocationEntity
	{
		public IntersectionCore(Intersection root, int distance = 10) : base(root, distance)
		{
		}

		internal override string BuildObjectName(string parentName)
		{
			return $".{nameof(IntersectionCore)}";
		}
	}
}
