using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.UnitTests
{
	public class UnitTest1
	{
		[Fact]
		public void RunSimulation_GivenSimpleIntersection_GivenOneCar_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			Intersection intersection = new Intersection()
			{
				EastLanes = new()
				{
					Distance = 10,
					LanesCollection =
					[
						new Lane()
					]
				},
				WestLanes = new()
				{
					Distance = 10,
					LanesCollection =
					[
						new Lane()
					]
				}
			};

			Simulation simulation
		}
	}
}