using FluentAssertions;
using TrafficSimulator.Application.Commons.Extensions;
using TrafficSimulator.Application.Commons.Helpers;

namespace TrafficSimulator.Application.UnitTests.Commons
{
	public class CircularListExtensionsTests
	{
		[Theory]
		[InlineData(new string[3] { "one", "two", "three" }, "two")]
		[InlineData(new string[3] { "one", "two", "three" }, "three")]
		[InlineData(new string[3] { "one", "two", "three" }, "one")]
		public void Set_ShouldSetCorrentItem(IEnumerable<string> inputCollection, string elementToSet)
		{
			CircularList<string> circularList = new CircularList<string>(inputCollection);

			circularList.Set(elementToSet, s => s);
			circularList.Current.Should().Be(elementToSet);
		}

		[Theory]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 2, "aa")]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 3, "aaa")]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 1, "a")]
		public void Set_WithConversionFromStringToInt_ShouldSetCorrentItem(IEnumerable<string> inputCollection, int elementToSet, string expectedElement)
		{
			CircularList<string> circularList = new CircularList<string>(inputCollection);

			circularList.Set(elementToSet, s => s.Length);
			circularList.Current.Should().Be(expectedElement);
		}
	}
}
