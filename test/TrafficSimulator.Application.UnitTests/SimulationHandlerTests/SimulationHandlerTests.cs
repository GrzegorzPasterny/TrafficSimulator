using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Infrastructure.Intersections;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class SimulationHandlerTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public SimulationHandlerTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void RunSimulation_GivenSimpleIntersection_GivenOneCar_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			Intersection intersection = new Intersection()
			{
				EastLanes = new()
				{
					Distance = 10,
					InboundLanes =
					[
						new Lane()
					],
					OutboundLanes =
					[
						new Lane()
					]
				},
				WestLanes = new()
				{
					Distance = 10,
					InboundLanes =
					[
						new Lane()
					],
					OutboundLanes =
					[
						new Lane()
					]
				}
			};

			IIntersectionRepository intersectionRepository = new IntersectionManager();

			ISimulationHandler simulationHandler = new SimulationHandler(intersectionRepository);

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			ErrorOr<SimulationState> state;

			do
			{
				state = simulationHandler.GetState();

				state.IsError.Should().BeFalse();


			} while (!state.Value.HasFinished);

			// TODO: Print final results
		}
	}
}