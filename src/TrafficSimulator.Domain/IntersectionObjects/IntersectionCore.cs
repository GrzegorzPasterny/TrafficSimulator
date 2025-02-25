using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class IntersectionCore : LocationEntity
	{
		public IntersectionCore(Intersection root, IntersectionObject? parent, int distance = 10) : base(root, parent, distance)
		{
		}

		public override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(IntersectionCore)}";
		}
	}
}
