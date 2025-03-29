using FluentAssertions;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.ModelsTests
{
	public class TrafficLightsTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private const int _orangeLightTimespanMs = 5000;
		private readonly IntersectionSimulation _intersectionSimulation;
		private Intersection _intersection => _intersectionSimulation.Intersection;
		private readonly TrafficLight _trafficLights;

		public TrafficLightsTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			InboundLane sampleInboundLane = _intersection.LanesCollection.First().InboundLanes!.First();

			_trafficLights = new TrafficLight(_intersection, sampleInboundLane, (options) =>
			{
				options.SwitchLightTimespanMs = TimeSpan.FromMilliseconds(_orangeLightTimespanMs);
				options.InitialState = TrafficLightState.Red;
			});
		}

		[Fact]
		public void ChangeTrafficLightsToGreen_WhenRedLightIsSetInitially_ShouldChangeTheLight()
		{
			_trafficLights.IsOn.Should().BeTrue();

			_trafficLights.SwitchToGreen().IsSuccess.Should().BeTrue();

			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Green);
		}

		[Fact]
		public void ChangeTrafficLightsToRed_WhenRedLightIsSetInitially_ShouldReturnError()
		{
			_trafficLights.IsOn.Should().BeTrue();

			_trafficLights.SwitchToRed().IsSuccess.Should().BeTrue();

			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Red);
		}

		[Fact]
		public void ChangeTrafficLights_WhenTrafficLightsAreTurnedOff_ShouldReturnError()
		{
			_trafficLights.TurnOff();

			_trafficLights.SwitchToRed().IsFailure.Should().BeTrue();

			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Off);
		}
	}
}
