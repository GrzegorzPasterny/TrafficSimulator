using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane
	{
		public InboundLane(Intersection root, IntersectionObject? parent, LaneType[] laneTypes, bool addTrafficLights = true, int distance = 10)
			: base(root, parent, distance)
		{
			foreach (var laneType in laneTypes)
			{
				TurnPossibility turnPossibility = new TurnPossibility();

				turnPossibility.LaneType = laneType;
				turnPossibility.ContainsTrafficLights = addTrafficLights;

				if (addTrafficLights)
				{
					turnPossibility.TrafficLights = new TrafficLights(root, this);
				}

				TurnPossibilities.Add(turnPossibility);
			}
		}

		public ICarGenerator CarGenerator { get; set; }

		public List<TurnPossibility> TurnPossibilities { get; set; } = [];

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(InboundLane)}";
		}
	}
}
