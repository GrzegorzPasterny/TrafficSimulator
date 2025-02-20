using MediatR;
using Moq;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Infrastructure.UnitTests.CarGeneratorsTests
{
	public class SingleCarsGeneratorTests
	{


		[Fact]
		public async Task RunCarsGenerator_ShouldProduceOneCar()
		{
			// Arrange
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			// Arrange
			var mediatorMock = new Mock<IMediator>();
			var singleCarsGenerator = new SingleCarGenerator(intersection, inboundLane, mediatorMock.Object);

			// Act
			while (singleCarsGenerator.IsGenerationFinished == false)
			{
				_ = await singleCarsGenerator.Generate(TimeSpan.FromMicroseconds(100));
			}

			// Assert
			mediatorMock.Verify(m => m.Send(It.IsAny<AddCarCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
