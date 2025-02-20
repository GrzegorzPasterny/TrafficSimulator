using TrafficSimulator.Application.Commons.Helpers;

namespace TrafficSimulator.Application.UnitTests.Helpers;

public class CircularListTests
{
	[Fact]
	public void CircularList_InitialState_ShouldPointToFirstElement()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });
		Assert.Equal(10, list.Current);
	}

	[Fact]
	public void MoveNext_ShouldMoveToNextElement()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });

		list.MoveNext();
		Assert.Equal(20, list.Current);

		list.MoveNext();
		Assert.Equal(30, list.Current);
	}

	[Fact]
	public void MoveNext_ShouldWrapAroundToStart()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });

		list.MoveNext();
		list.MoveNext();
		list.MoveNext(); // Should wrap around

		Assert.Equal(10, list.Current);
	}

	[Fact]
	public void MovePrevious_ShouldMoveToPreviousElement()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });

		list.MoveNext();
		list.MoveNext();
		list.MovePrevious();

		Assert.Equal(20, list.Current);
	}

	[Fact]
	public void MovePrevious_ShouldWrapAroundToEnd()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });

		list.MovePrevious(); // Should wrap around to last element
		Assert.Equal(30, list.Current);
	}

	[Fact]
	public void Reset_ShouldMovePointerBackToStart()
	{
		var list = new CircularList<int>(new[] { 10, 20, 30 });

		list.MoveNext();
		list.MoveNext();
		list.Reset();

		Assert.Equal(10, list.Current);
	}

	[Fact]
	public void CircularList_WithSingleElement_ShouldAlwaysPointToSameElement()
	{
		var list = new CircularList<int>(new[] { 42 });

		list.MoveNext();
		Assert.Equal(42, list.Current);

		list.MovePrevious();
		Assert.Equal(42, list.Current);
	}

	[Fact]
	public void CircularList_EmptyList_ShouldThrowException()
	{
		Assert.Throws<ArgumentException>(() => new CircularList<int>(new List<int>()));
	}
}
