using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models.Intersection
{
	public class Lane : LocationEntity
	{
		public Lane(Intersection root, string name, LaneType[] laneTypes, WorldDirection worldDirection, int distance = 10) : base(root, name, distance)
		{
			LaneType = laneTypes;
			WorldDirection = worldDirection;
		}

		public LaneType[] LaneType { get; }
		public WorldDirection WorldDirection { get; }
		public ICarGenerator CarGenerator { get; set; }
	}
}
