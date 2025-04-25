using FluentAssertions;
using TrafficSimulator.Application.Commons.Extensions;
using TrafficSimulator.Application.Commons.Helpers;

namespace TrafficSimulator.Application.UnitTests.Commons
{
	public class CircularListExtensionsTests
	{
		[Theory]
		[InlineData(new string[3] { "one", "two", "three" }, "two", true)]
		[InlineData(new string[3] { "one", "two", "three" }, "three", true)]
		[InlineData(new string[3] { "one", "two", "three" }, "one", true)]
		[InlineData(new string[3] { "one", "two", "three" }, "four", false)]
		public void Set_ShouldSetCorrentItem(IEnumerable<string> inputCollection, string elementToSet, bool expectedResult)
		{
			CircularList<string> circularList = new(inputCollection);
			string initialCurrentItem = circularList.Current;

			circularList.Set(elementToSet, s => s).Should().Be(expectedResult);

			if (expectedResult)
			{
				circularList.Current.Should().Be(elementToSet);
			}
			else
			{
				circularList.Current.Should().Be(initialCurrentItem);
			}
		}

		[Theory]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 2, "aa", true)]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 3, "aaa", true)]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 1, "a", true)]
		[InlineData(new string[3] { "a", "aa", "aaa" }, 4, "aaaa", false)]
		public void Set_WithConversionFromStringToInt_ShouldSetCorrentItem(
			IEnumerable<string> inputCollection, int elementToSet, string expectedElement, bool expectedResult)
		{
			CircularList<string> circularList = new(inputCollection);
			string initialCurrentItem = circularList.Current;

			circularList.Set(elementToSet, s => s.Length).Should().Be(expectedResult);

			if (expectedResult)
			{
				circularList.Current.Should().Be(expectedElement);
			}
			else
			{
				circularList.Current.Should().Be(initialCurrentItem);
			}
		}
	}
}
