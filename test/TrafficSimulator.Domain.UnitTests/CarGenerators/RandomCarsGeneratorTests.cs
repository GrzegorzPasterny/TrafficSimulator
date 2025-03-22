using FluentAssertions;
using MediatR;
using Moq;
using Moq.Protected;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.UnitTests.CarGenerators
{
	public class RandomCarsGeneratorTests
	{
		Mock<Intersection> _rootMock = new Mock<Intersection>("MockIntersection", null);
		Mock<IntersectionObject> _parentMock;
		Mock<ISender> _mediatorMock = new Mock<ISender>();
		private Mock<RandomCarsGenerator>? _randomCarsGeneratorMock;

		public RandomCarsGeneratorTests()
		{
			_parentMock = new Mock<IntersectionObject>(_rootMock.Object, _rootMock.Object, "MockParent");
		}

		[Fact]
		public async Task GenerateCars_WithMaximumProbability_WhenOneCarIsRequested_ShouldProduceCarImmediately()
		{
			RandomCarsGeneratorOptions options = new RandomCarsGeneratorOptions()
			{
				CarOptions = new CarOptions(),
				AmountOfCarsToGenerate = 1,
				BaseRate = 1
			};

			_randomCarsGeneratorMock = new Mock<RandomCarsGenerator>(
				_rootMock.Object, _parentMock.Object, _mediatorMock.Object, options)
			{
				CallBase = true
			};

			_randomCarsGeneratorMock.Protected()
				.Setup<Task>("GenerateCar")
				.Returns(Task.CompletedTask);

			RandomCarsGenerator randomCarsGenerator = _randomCarsGeneratorMock.Object;

			(await randomCarsGenerator.Generate(TimeSpan.FromSeconds(2))).IsSuccess.Should().BeTrue();

			randomCarsGenerator.IsGenerationCompleted.Should().BeTrue();
			_randomCarsGeneratorMock.Protected()
				.Verify("GenerateCar", Times.Once());
		}

		[Fact]
		public async Task GenerateCars_WithThreeRequests_WithMaximumProbability_ShouldGenerateThreeCars()
		{
			// Arrange
			RandomCarsGeneratorOptions options = new RandomCarsGeneratorOptions()
			{
				CarOptions = new CarOptions(),
				AmountOfCarsToGenerate = 3,
				BaseRate = 1
			};

			_randomCarsGeneratorMock = new Mock<RandomCarsGenerator>(
				_rootMock.Object, _parentMock.Object, _mediatorMock.Object, options)
			{
				CallBase = true
			};

			_randomCarsGeneratorMock.Protected()
				.Setup<Task>("GenerateCar")
				.Returns(Task.CompletedTask);

			RandomCarsGenerator randomCarsGenerator = _randomCarsGeneratorMock.Object;

			// Act
			for (int i = 0; i < options.AmountOfCarsToGenerate; i++)
			{
				(await randomCarsGenerator.Generate(TimeSpan.FromSeconds(2))).IsSuccess.Should().BeTrue();
			}

			// Assert
			randomCarsGenerator.IsGenerationCompleted.Should().BeTrue();
			_randomCarsGeneratorMock.Protected()
				.Verify("GenerateCar", Times.Exactly(3));
		}

		[Fact]
		public async Task GenerateCars_WithMultipleRequests_ShouldGenerateCorrectNumberOfCars()
		{
			// Arrange
			RandomCarsGeneratorOptions options = new RandomCarsGeneratorOptions()
			{
				CarOptions = new CarOptions(),
				AmountOfCarsToGenerate = 1_000,
				BaseRate = 0.5
			};

			_randomCarsGeneratorMock = new Mock<RandomCarsGenerator>(
				_rootMock.Object, _parentMock.Object, _mediatorMock.Object, options)
			{
				CallBase = true
			};

			_randomCarsGeneratorMock.Protected()
				.Setup<Task>("GenerateCar")
				.Returns(Task.CompletedTask);

			RandomCarsGenerator randomCarsGenerator = _randomCarsGeneratorMock.Object;

			// Act
			for (int i = 0; i < 1_000; i++)
			{
				(await randomCarsGenerator.Generate(TimeSpan.FromSeconds(1))).IsSuccess.Should().BeTrue();
			}

			// Assert
			randomCarsGenerator.IsGenerationCompleted.Should().BeFalse();
			_randomCarsGeneratorMock.Protected()
				.Verify("GenerateCar", Times.Between(400, 600, Moq.Range.Inclusive));
		}
	}
}
