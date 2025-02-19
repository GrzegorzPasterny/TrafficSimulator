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
			// Arrange
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();

			IEnumerable<Domain.Models.IntersectionObjects.TrafficLights> trafficLights =
				intersection.ObjectLookup.OfType<Domain.Models.IntersectionObjects.TrafficLights>();

			// Take all Inbound lanes and all turn possibilities and add them as a default Traffic Phase
			TrafficPhase allGreenTrafficPhase = new TrafficPhase("AllGreen", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Green);

			trafficPhasesHandler.TrafficPhases.Add(allGreenTrafficPhase);

			// All Inbound lanes and all turn possibilities have red light
			TrafficPhase allRedTrafficPhase = new TrafficPhase("AllRed", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Red);

			trafficPhasesHandler.TrafficPhases.Add(allRedTrafficPhase);

			// Act
			trafficPhasesHandler.SetPhase("AllGreen");

			// Assert
			trafficLights.All(lights => lights.TrafficLightState is TrafficLightState.Green);

			// Act
			trafficPhasesHandler.SetPhase("AllRed");

			// Assert
			trafficLights.All(lights => lights.TrafficLightState is TrafficLightState.Red);
		}
	}
}
