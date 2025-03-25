using FluentAssertions;
using System.Reflection;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.Cars;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Application.UnitTests.Traffic
{
	public class SimpleDynamicTrafficLightsHandlerTests
	{
		[Fact]
		public async Task SetLights_ShouldChooseTheCorrectTrafficPhase()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane eastInboundLane = intersection.LanesCollection
				.Single(l => l.WorldDirection == WorldDirection.East)
				.InboundLanes[0];
			InboundLane westInboundLane = intersection.LanesCollection
				.Single(l => l.WorldDirection == WorldDirection.West)
				.InboundLanes[0];

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);
			CarsRepositoryInMemory carsRepositoryInMemory = new CarsRepositoryInMemory();

			var isCarWaitingProperty = typeof(Car).GetProperty("IsCarWaiting",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			for (int i = 0; i < 2; i++)
			{
				var car = new Car(eastInboundLane);
				isCarWaitingProperty.SetValue(car, true);
				await carsRepositoryInMemory.AddCarAsync(car);
			}

			for (int i = 0; i < 1; i++)
			{
				var car = new Car(westInboundLane);
				isCarWaitingProperty.SetValue(car, true);
				await carsRepositoryInMemory.AddCarAsync(car);
			}
			carsRepositoryInMemory.CommitPendingCars();

			SimpleDynamicTrafficLightsHandler handler =
				new SimpleDynamicTrafficLightsHandler(carsRepositoryInMemory, trafficPhasesHandler)
				{
					MinimalTimeForOnePhase = TimeSpan.FromSeconds(1.5)
				};

			handler.LoadIntersection(intersection);
			TrafficPhase initialTrafficPhase = handler.GetCurrentTrafficPhase();

			// Act
			_ = await handler.SetLights(TimeSpan.FromSeconds(1));

			handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo(initialTrafficPhase.Name);

			_ = await handler.SetLights(TimeSpan.FromSeconds(1));

			handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForEastOnly");

			for (int i = 0; i < 2; i++)
			{
				var car = new Car(westInboundLane);
				isCarWaitingProperty.SetValue(car, true);
				await carsRepositoryInMemory.AddCarAsync(car);
			}
			carsRepositoryInMemory.CommitPendingCars();

			_ = await handler.SetLights(TimeSpan.FromSeconds(2));

			handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForWestOnly");
		}
	}
}
