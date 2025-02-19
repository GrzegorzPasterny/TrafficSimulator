using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

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
	}
}
