using FluentAssertions;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Application.UnitTests.Traffic
{
	public class TrafficPhasesHandlerTests
	{
		[Fact]
		public void ChangePhases_WithSmallTimeIncrement_ShouldTurnTheCorrectLightsGreenOrangeAndRed()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			TrafficPhasesHandler trafficPhasesHandler = new()
			{
				LightsChangeDuration = TimeSpan.FromSeconds(0.5),
			};

			trafficPhasesHandler.LoadIntersection(intersection);

			IEnumerable<TrafficLight> trafficLights =
				intersection.ObjectLookup.OfType<TrafficLight>();

			// Take all Inbound lanes and all turn possibilities and add them as a default Traffic Phase
			TrafficPhase allGreenTrafficPhase = new("AllGreen", intersection);
			allGreenTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Green);

			// All Inbound lanes and all turn possibilities have red light
			TrafficPhase allRedTrafficPhase = new("AllRed", intersection);
			allRedTrafficPhase.TrafficLightsAssignments.ForEach(a => a.TrafficLightState = TrafficLightState.Red);

			// Act
			trafficPhasesHandler.SetPhase("AllGreen", TimeSpan.Zero);

			// Assert
			trafficLights.Select(trafficLight => trafficLight.TrafficLightState)
				.Should().AllBeEquivalentTo(TrafficLightState.Orange);

			// Act
			trafficPhasesHandler.SetPhase("AllGreen", TimeSpan.FromSeconds(0.1));

			// Assert
			trafficLights.Select(trafficLight => trafficLight.TrafficLightState)
				.Should().AllBeEquivalentTo(TrafficLightState.Orange);

			// Act
			trafficPhasesHandler.SetPhase("AllGreen", TimeSpan.FromSeconds(1));

			// Assert
			trafficLights.Select(trafficLight => trafficLight.TrafficLightState)
				.Should().AllBeEquivalentTo(TrafficLightState.Green);

			// Act
			trafficPhasesHandler.SetPhase("AllRed", TimeSpan.FromSeconds(1));

			// Assert
			trafficLights.Select(trafficLight => trafficLight.TrafficLightState)
				.Should().AllBeEquivalentTo(TrafficLightState.Red);
		}
	}
}
