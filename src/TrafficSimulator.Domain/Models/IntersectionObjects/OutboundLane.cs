using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class OutboundLane : LocationEntity
	{
		public OutboundLane(Intersection root, IntersectionObject? parent, int distance = 10)
			: base(root, parent, distance)
		{
		}

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(OutboundLane)}";
		}
	}
}
