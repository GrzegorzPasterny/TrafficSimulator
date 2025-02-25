using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.IntersectionObjects;
using TrafficSimulator.Domain.IntersectionObjects.IntersectionProperties;

namespace TrafficSimulator.Application.UnitTests.Commons
{
    public static class TrafficPhasesRespository
	{
		public static TrafficPhase AllLightsGreen(Intersection intersection)
		{
			TrafficPhase allGreenTrafficPhase = new TrafficPhase("AllGreen", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Green);

			return allGreenTrafficPhase;
		}

		public static TrafficPhase AllLightsRed(Intersection intersection)
		{
			TrafficPhase allRedTrafficPhase = new TrafficPhase("AllRed", intersection);
			allRedTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Red);

			return allRedTrafficPhase;
		}

		public static TrafficPhase GreenForOneDirection(Intersection intersection, WorldDirection worldDirection)
		{
			TrafficPhase greenForOneDirectionTrafficPhase = new TrafficPhase($"GreenFor{worldDirection}Only", intersection);
			greenForOneDirectionTrafficPhase.TrafficLightsAssignments.ForEach(a =>
			{
				if (a.InboundLane.WorldDirection == worldDirection)
				{
					a.TrafficLightState = TrafficLightState.Green;
				}
				else
				{
					a.TrafficLightState = TrafficLightState.Red;
				}
			});

			return greenForOneDirectionTrafficPhase;
		}
	}
}
