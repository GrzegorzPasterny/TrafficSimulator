using FluentAssertions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.UnitTests.Commons;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.ModelsTests
{
	public class CarTests : TestsBase
	{
		public CarTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Theory]
		[InlineData(10, 5, 100, 60)]
		[InlineData(2, 5, 100, 12)]
		[InlineData(10, 10, 1000, 3)]
		[InlineData(10, 9, 1000, 4)]
		[InlineData(1, 1, 1, 3000)]
		public void CarMoves_CurrentLocationShouldBeCalculatedProperly(
			int locationDistance, int velocity, int timespanMs, int amountOfMovesExpected)
		{
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			InboundLane? carStartLocation = intersection.GetObject<InboundLane>((lane) => true);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(timespanMs);

			carStartLocation.Should().NotBeNull();

			// Default Traffic Lights are Red, that's why They have to be set to Green
			carStartLocation!.TurnPossibilities.First().TrafficLights!.SwitchToGreen();

			Car car = new Car(carStartLocation!);
			car.Velocity = velocity;
			car.DistanceToCover.ForEach(l => l.Distance = locationDistance);

			while (car.HasReachedDestination == false)
			{
				_logger.LogInformation("{Car}", car);
				car.Move(timeSpan);
			}

			_logger.LogInformation("{Car}", car);
			car.MovesSoFar.Should().Be(amountOfMovesExpected);
			car.Move(timeSpan).IsFailure.Should().BeTrue();
		}
	}
}
