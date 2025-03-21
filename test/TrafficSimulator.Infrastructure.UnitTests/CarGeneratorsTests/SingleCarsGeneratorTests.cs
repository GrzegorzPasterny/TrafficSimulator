using MediatR;
using Moq;
using TrafficSimulator.Domain.CarGenerators.DomainEvents;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Infrastructure.UnitTests.CarGeneratorsTests
{
	public class SingleCarsGeneratorTests
	{
		[Fact]
		public async Task RunCarsGenerator_ShouldProduceOneCar()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			var mediatorMock = new Mock<IMediator>();
			var singleCarsGenerator = new SingleCarGenerator(intersection, inboundLane, mediatorMock.Object);

			// Act
			while (singleCarsGenerator.IsGenerationCompleted == false)
			{
				_ = await singleCarsGenerator.Generate(TimeSpan.FromMicroseconds(100));
			}

			// Assert
			mediatorMock.Verify(m => m.Send(It.IsAny<AddCarCommandDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
