using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.ModelsTests
{
	public class CarTests
	{
		internal readonly ILogger<CarTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;

		public CarTests(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog(logger, dispose: true);
			});

			_logger = _loggerFactory.CreateLogger<CarTests>();
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
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane? carStartLocation = intersection.GetObject<InboundLane>((lane) => true);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(timespanMs);

			carStartLocation.Should().NotBeNull();

			// Default Traffic Lights are Red, that's why They have to be set to Green
			carStartLocation!.TrafficLights!.SwitchToGreen();

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
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane? carStartLocation = intersection.GetObject<InboundLane>((lane) => true);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(100);

			carStartLocation.Should().NotBeNull();
			List<Car> cars = new List<Car>();

			// Make car jam before zebra
			carStartLocation!.TrafficLights!.SwitchToRed();

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
			carStartLocation!.TrafficLights!.SwitchToGreen();

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
