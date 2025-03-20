using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Domain.UnitTests.CommonsTests;

public class WorldDirectionExtensionsTests
{
	[Theory]
	[InlineData(WorldDirection.North, LaneType.Straight, WorldDirection.South)]
	[InlineData(WorldDirection.North, LaneType.Right, WorldDirection.West)]
	[InlineData(WorldDirection.North, LaneType.Left, WorldDirection.East)]
	[InlineData(WorldDirection.North, LaneType.TurnAround, WorldDirection.North)]

	[InlineData(WorldDirection.East, LaneType.Straight, WorldDirection.West)]
	[InlineData(WorldDirection.East, LaneType.Right, WorldDirection.North)]
	[InlineData(WorldDirection.East, LaneType.Left, WorldDirection.South)]
	[InlineData(WorldDirection.East, LaneType.TurnAround, WorldDirection.East)]

	[InlineData(WorldDirection.South, LaneType.Straight, WorldDirection.North)]
	[InlineData(WorldDirection.South, LaneType.Right, WorldDirection.East)]
	[InlineData(WorldDirection.South, LaneType.Left, WorldDirection.West)]
	[InlineData(WorldDirection.South, LaneType.TurnAround, WorldDirection.South)]

	[InlineData(WorldDirection.West, LaneType.Straight, WorldDirection.East)]
	[InlineData(WorldDirection.West, LaneType.Right, WorldDirection.South)]
	[InlineData(WorldDirection.West, LaneType.Left, WorldDirection.North)]
	[InlineData(WorldDirection.West, LaneType.TurnAround, WorldDirection.West)]
	public void Rotate_ShouldReturnExpectedDirection(WorldDirection start, LaneType laneType, WorldDirection expected)
	{
		// Act
		var result = start.Rotate(laneType);

		// Assert
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Rotate_InvalidLaneType_ShouldThrowArgumentOutOfRangeException()
	{
		// Arrange
		var invalidLaneType = (LaneType)999;

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => WorldDirection.North.Rotate(invalidLaneType));
	}
}