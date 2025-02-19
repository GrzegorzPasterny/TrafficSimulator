using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Commons.Helpers
{
	public static class IntersectionHelper
	{
		public static IEnumerable<TurnWithTrafficLight> GetAllTurnDefinitionsWithTrafficLights(this Intersection intersection)
		{
			List<TurnWithTrafficLight> turnsWithTrafficLight = [];
			IEnumerable<InboundLane> inboundLanes = (IEnumerable<InboundLane>)intersection.ObjectLookup.Where((o) => o is InboundLane);

			foreach (InboundLane inboundLane in inboundLanes)
			{
				List<TurnWithTrafficLight> turnsOnTheInboundLane = [];

				foreach (TurnPossibility turnPossibility in inboundLane.TurnPossibilities)
				{
					TurnWithTrafficLight turn = new();

					turn.InboundLane = inboundLane;
					turn.TurnPossibility = turnPossibility;
					// As a default value
					turn.TrafficLightState = Models.Lights.TrafficLightState.Red;

					turnsWithTrafficLight.Add(turn);
				}
			}

			return turnsWithTrafficLight;
		}
	}
}
