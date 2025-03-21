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
		public async Task GenerateCars_Should()
		{
			RandomCarsGeneratorOptions options = new RandomCarsGeneratorOptions()
			{
				CarOptions = new CarOptions(),
				AmountOfCarsToGenerate = 1,
				Probability = 100
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

			(await randomCarsGenerator.Generate(TimeSpan.Zero)).IsSuccess.Should().BeTrue();

			randomCarsGenerator.IsGenerationCompleted.Should().BeTrue();
			_randomCarsGeneratorMock.Protected()
				.Verify("GenerateCar", Times.Once());
		}
	}
}
