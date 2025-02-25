using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane
	{
		public InboundLane(
			Intersection root, IntersectionObject? parent, LaneType[] laneTypes, WorldDirection worldDirection,
			string name = "", bool addTrafficLights = true, int distance = 10)
			: base(root, parent, worldDirection, name, distance)
		{
			TurnPossibilities = (IReadOnlyList<TurnPossibility>)laneTypes.ToList().Select(laneType =>
			{
				TurnPossibility turnPossibility = new TurnPossibility();

				turnPossibility.LaneType = laneType;
				turnPossibility.ContainsTrafficLights = addTrafficLights;

				if (addTrafficLights)
				{
					turnPossibility.TrafficLights = new TrafficLights(root, this);
				}

				return turnPossibility;
			});
		}

		public ICarGenerator? CarGenerator { get; set; }

		public IReadOnlyList<TurnPossibility> TurnPossibilities { get; }
	}
}
