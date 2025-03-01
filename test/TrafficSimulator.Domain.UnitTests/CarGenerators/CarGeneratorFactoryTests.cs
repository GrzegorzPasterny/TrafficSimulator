using FluentAssertions;
using MediatR;
using Moq;
using System.Text.Json;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Domain.UnitTests.CarGenerators;

public class CarGeneratorFactoryTests
{
	private readonly Mock<ISender> _mediatorMock;
	private readonly CarGeneratorFactory _factory;
	private readonly Intersection _root;
	private readonly IntersectionObject _parent;

	public CarGeneratorFactoryTests()
	{
		_mediatorMock = new Mock<ISender>();
		_factory = new CarGeneratorFactory(_mediatorMock.Object);
		_root = IntersectionsRepository.ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights.Intersection;
		_parent = _root.LanesCollection[0].InboundLanes![0];
	}

	[Fact]
	public void Create_ShouldReturnSingleCarGenerator_WhenValidInput()
	{
		// Arrange
		var options = new SingleCarGeneratorOptions { DelayForGeneratingTheCar = TimeSpan.FromMilliseconds(250) };
		var optionsJson = JsonSerializer.Serialize(options);

		// Act
		var result = _factory.Create(nameof(SingleCarGenerator), JsonSerializer.Deserialize<JsonElement>(optionsJson), _root, _parent);

		// Assert
		Assert.False(result.IsError);
		Assert.IsType<SingleCarGenerator>(result.Value);
	}

	[Fact]
	public void Create_ShouldReturnMultipleCarsGenerator_WhenValidInput()
	{
		// Arrange
		var options = new MultipleCarsGeneratorOptions { AmountOfCarsToGenerate = 5, DelayBetweenCarGeneration = TimeSpan.FromSeconds(1) };
		var optionsJson = JsonSerializer.Serialize(options);

		// Act
		var result = _factory.Create(nameof(MultipleCarsGenerator), JsonSerializer.Deserialize<JsonElement>(optionsJson), _root, _parent);

		// Assert
		Assert.False(result.IsError);
		Assert.IsType<MultipleCarsGenerator>(result.Value);
	}

	[Fact]
	public void Create_ShouldReturnNullValue_WhenCarGeneratorTypeIsNullOrEmpty()
	{
		// Act
		CarGeneratorOptions options = new CarGeneratorOptions();
		var optionsJson = JsonSerializer.Serialize(options);


		var result1 = _factory.Create(null!, JsonSerializer.Deserialize<JsonElement>(optionsJson), _root, _parent);
		var result2 = _factory.Create("", JsonSerializer.Deserialize<JsonElement>(optionsJson), _root, _parent);

		// Assert
		result1.IsError.Should().BeFalse();
		result2.IsError.Should().BeFalse();

		result1.Value.Should().BeNull();
		result2.Value.Should().BeNull();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenCarGeneratorTypeIsUnknown()
	{
		// Arrange
		CarGeneratorOptions options = new CarGeneratorOptions();
		var optionsJson = JsonSerializer.Serialize(options);

		// Act & Assert
		var ex = Assert.Throws<Exception>(() => _factory.Create("UnknownGenerator", JsonSerializer.Deserialize<JsonElement>(optionsJson), _root, _parent));
		Assert.Contains("Unknown CarGenerator type", ex.Message);
	}
}
