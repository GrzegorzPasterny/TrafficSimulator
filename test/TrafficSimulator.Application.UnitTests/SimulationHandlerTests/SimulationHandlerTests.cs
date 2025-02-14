using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain.Commons;
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
			IntersectionCore intersectionCore = new()
			{
				Distance = 10,
			};

			Intersection intersection = new Intersection()
			{
				IntersectionCore = intersectionCore,
				Lanes =
				[
					new(WorldDirection.West)
					{
						Distance = 10,
						InboundLanes =
						[
							new Lane(intersectionCore, LaneTypeHelper.Straight(), WorldDirection.West)
						],
						OutboundLanes =
						[
							new Lane(intersectionCore, LaneTypeHelper.Straight(), WorldDirection.West)
						]
					},
					new(WorldDirection.East)
					{
						Distance = 10,
						InboundLanes =
						[
							new Lane(intersectionCore, LaneTypeHelper.Straight(), WorldDirection.East)
						],
						OutboundLanes =
						[
							new Lane(intersectionCore, LaneTypeHelper.Straight(), WorldDirection.East)
						]
					}
				]
			};

			IIntersectionRepository intersectionRepository = new IntersectionManager();

			ISimulationHandler simulationHandler = new IntersectionSimulationHandler(intersectionRepository);

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