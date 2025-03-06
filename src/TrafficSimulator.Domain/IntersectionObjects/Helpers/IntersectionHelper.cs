using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Commons.Helpers
{
	public static class IntersectionHelper
	{
		public static IEnumerable<TurnWithTrafficLight> GetAllTurnDefinitionsWithTrafficLights(this Intersection intersection)
		{
			List<TurnWithTrafficLight> turnsWithTrafficLight = [];
			IEnumerable<InboundLane> inboundLanes = intersection.ObjectLookup.OfType<InboundLane>();

			foreach (InboundLane inboundLane in inboundLanes)
			{
				List<TurnWithTrafficLight> turnsOnTheInboundLane = [];

				foreach (LaneType laneType in inboundLane.LaneTypes)
				{
					TurnWithTrafficLight turn = new();

					turn.InboundLane = inboundLane;
					turn.TurnPossibility = new TurnPossibility()
					{
						ContainsTrafficLights = inboundLane.ContainsTrafficLights,
						LaneType = laneType,
						TrafficLights = inboundLane.TrafficLights
					};
					// As a default value
					turn.TrafficLightState = Models.Lights.TrafficLightState.Red;

					turnsWithTrafficLight.Add(turn);
				}
			}

			return turnsWithTrafficLight;
		}
	}
}
