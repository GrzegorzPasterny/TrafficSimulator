using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.UnitTests.Traffic
{
	public class TrafficPhasesHandlerTests
	{
		[Fact]
		public void ChangePhases_ShouldTurnTheCorrectLightsGreenAndRed()
		{
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();

			// Take all Inbound lanes and all turn possibilities and add them as a default Traffic Phase
			TrafficPhase allGreenTrafficPhase = new TrafficPhase("AllGreen", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Green);

			trafficPhasesHandler.TrafficPhases.Add(allGreenTrafficPhase);

			// All Inbound lanes and all turn possibilities have red light
			TrafficPhase allRedTrafficPhase = new TrafficPhase("AllRed", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Red);

			trafficPhasesHandler.TrafficPhases.Add(allRedTrafficPhase);

			trafficPhasesHandler.SetPhase("AllGreen");

			// Assert
			// TODO

			trafficPhasesHandler.SetPhase("AllRed");
		}
	}
}
