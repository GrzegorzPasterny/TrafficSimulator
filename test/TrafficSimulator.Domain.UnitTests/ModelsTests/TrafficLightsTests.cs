using FluentAssertions;
using TrafficSimulator.Domain.Models;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.ModelsTests
{
	public class TrafficLightsTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TrafficLightsTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void ChangeTrafficLights_ShouldChangeLightsProperly()
		{
			// Arrange
			int orangeLightTimespanMs = 5000;

			TrafficLights trafficLights = new TrafficLights((options) =>
			{
				options.SwitchLightTimespanMs = TimeSpan.FromMilliseconds(orangeLightTimespanMs);
				options.InitialState = TrafficLightState.Red;
			});

			// Act & Assert
			// Not allowed to change the state when demanded state is already applied
			trafficLights.SwitchToRed().IsFailure.Should().BeTrue();

			// Normal lights state change request
			trafficLights.SwitchToGreen().IsSuccess.Should().BeTrue();

			// TODO: When changing to orange it implemented
			// After change request we first get orange light
			//trafficLights.TrafficLightState.Should().Be(TrafficLightState.Orange);

			// Waiting for Ornage light to pass
			//await Task.Delay(orangeLightTimespanMs);

			// Light should finally change the state
			trafficLights.TrafficLightState.Should().Be(TrafficLightState.Green);
		}
	}
}
