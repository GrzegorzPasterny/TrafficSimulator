using FluentAssertions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.UnitTests.Commons;
using TrafficSimulator.Tests.Commons.Assets;
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
			car.MoveVelocity = velocity;
			car.DistanceToCover.ForEach(l => l.Distance = locationDistance);

			while (car.HasReachedDestination == false)
			{
				_logger.LogInformation("{Car}", car);
				car.Move(timeSpan, [car]);
			}

			_logger.LogInformation("{Car}", car);
			car.MovesSoFar.Should().Be(amountOfMovesExpected);
			car.Move(timeSpan, [car]).IsFailure.Should().BeTrue();
		}

		[Fact]
		public void CarsMove_WithOtherCarsOnTheWayOnRedLight_ShouldMoveWithoutCollisions()
		{
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			InboundLane? carStartLocation = intersection.GetObject<InboundLane>((lane) => true);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(100);

			carStartLocation.Should().NotBeNull();
			List<Car> cars = new List<Car>();

			// Make car jam before zebra
			carStartLocation!.TurnPossibilities.First().TrafficLights!.SwitchToRed();

			// Make 3 cars
			for (int i = 0; i < 3; i++)
			{
				Car car = new Car(carStartLocation!);
				cars.Add(car);
				car.Move(timeSpan, cars);
				_logger.LogInformation("{Car}", car);
			}

			int maxMoveCount = 10;
			int moveCount = 0;

			// Wait until all cars are waiting on red light
			while (cars.All(c => c.IsCarWaiting) == false && moveCount < maxMoveCount)
			{
				moveCount++;

				cars.ForEach(car =>
				{
					car.Move(timeSpan, cars);
					_logger.LogInformation("{Car}", car);
				});
			}

			cars.All(c => c.IsCarWaiting).Should().BeTrue();

			// Each car should be at different distance, as each car has non-zero Length and there is minimal distance between the cars
			cars.Select(car => car.CurrentLocation.DistanceLeft).Distinct().Count().Should().Be(cars.Count());

			// Relief the cars
			carStartLocation!.TurnPossibilities.First().TrafficLights!.SwitchToGreen();
			_logger.LogInformation("{TurnPossibility}", carStartLocation!.TurnPossibilities.First());

			moveCount = 0;

			// Wait until all cars are waiting on red light
			while (cars.All(c => c.HasReachedDestination) == false)
			{
				moveCount++;

				if (moveCount > maxMoveCount)
				{
					break;
				}

				cars.ForEach(car =>
				{
					car.Move(timeSpan, cars);
					_logger.LogInformation("{Car}", car);
				});
			}

			cars.All(car => car.HasReachedDestination).Should().BeTrue();
		}
	}
}
