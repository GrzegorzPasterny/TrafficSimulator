using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class Lane : LocationEntity
	{
		public Lane(IntersectionCore intersectionCore, LaneType[] laneTypes, WorldDirection worldDirection, int distance = 10) : base(distance)
		{
			IntersectionCore = intersectionCore;
			LaneType = laneTypes;
			WorldDirection = worldDirection;
		}

		public IntersectionCore IntersectionCore { get; }

		public LaneType[] LaneType { get; }
		public WorldDirection WorldDirection { get; }
	}
}
