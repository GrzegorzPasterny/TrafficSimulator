using FluentAssertions;
using TrafficSimulator.Domain.IntersectionObjects;
using TrafficSimulator.Domain.IntersectionObjects.IntersectionProperties;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.ModelsTests
{
    public class TrafficLightsTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private const int _orangeLightTimespanMs = 5000;
		private readonly Intersection _intersection;
		private readonly TrafficLights _trafficLights;

		public TrafficLightsTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			InboundLane sampleInboundLane = _intersection.LanesCollection.First().InboundLanes!.First();

			_trafficLights = new TrafficLights(_intersection, sampleInboundLane, (options) =>
			{
				options.SwitchLightTimespanMs = TimeSpan.FromMilliseconds(_orangeLightTimespanMs);
				options.InitialState = TrafficLightState.Red;
			});
		}

		[Fact]
		public void ChangeTrafficLightsToGreen_WhenRedLightIsSetInitially_ShouldChangeTheLight()
		{
			_trafficLights.IsOn.Should().BeTrue();

			// Normal lights state change request
			_trafficLights.SwitchToGreen().IsSuccess.Should().BeTrue();

			// TODO: When changing to orange it implemented
			// After change request we first get orange light
			//trafficLights.TrafficLightState.Should().Be(TrafficLightState.Orange);

			// Waiting for Ornage light to pass
			//await Task.Delay(orangeLightTimespanMs);

			// Light should finally change the state
			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Green);
		}

		[Fact]
		public void ChangeTrafficLightsToRed_WhenRedLightIsSetInitially_ShouldReturnError()
		{
			_trafficLights.IsOn.Should().BeTrue();

			// Not allowed to change the state when demanded state is already applied
			_trafficLights.SwitchToRed().IsFailure.Should().BeTrue();

			// Traffic lights state should be still the same
			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Red);
		}

		[Fact]
		public void ChangeTrafficLights_WhenTrafficLightsAreTurnedOff_ShouldReturnError()
		{
			_trafficLights.TurnOff();

			// Not allowed to change the state when demanded state is already applied
			_trafficLights.SwitchToRed().IsFailure.Should().BeTrue();

			// Traffic lights state should be still the same
			_trafficLights.TrafficLightState.Should().Be(TrafficLightState.Off);
		}
	}
}
