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
					IncommingLanes =
					[
						new Lane()
					]
				},
				WestLanes = new()
				{
					Distance = 10,
					IncommingLanes =
					[
						new Lane()
					]
				}
			};

			Simulation simulation
		}
	}
}