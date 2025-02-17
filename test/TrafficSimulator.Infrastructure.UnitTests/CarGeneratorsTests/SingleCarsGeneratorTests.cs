using ErrorOr;
using FluentAssertions;
using MediatR;
using Moq;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;

namespace TrafficSimulator.Infrastructure.UnitTests.CarGeneratorsTests
{
	public class SingleCarsGeneratorTests
	{


		[Fact]
		public async Task RunCarsGenerator_ShouldProduceOneCar()
		{
			// Arrange
			ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create()
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddLane(WorldDirection.East, true)
				//.AddCarGenerator()
				.AddLane(WorldDirection.East, false)
				.AddLanesCollection(WorldDirection.West)
				.AddLane(WorldDirection.West, true)
				.AddLane(WorldDirection.West, false)
				.Build();

			intersectionResult.IsError.Should().BeFalse();

			Intersection intersection = intersectionResult.Value;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			// Arrange
			var mediatorMock = new Mock<IMediator>();
			var singleCarsGenerator = new SingleCarGenerator(intersection, inboundLane, mediatorMock.Object);

			// Act
			singleCarsGenerator.StartGenerating();

			while (singleCarsGenerator.IsGenerationFinished().Value == false)
			{
				await Task.Delay(100);
			}

			// Assert
			mediatorMock.Verify(m => m.Send(It.IsAny<AddCarCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
