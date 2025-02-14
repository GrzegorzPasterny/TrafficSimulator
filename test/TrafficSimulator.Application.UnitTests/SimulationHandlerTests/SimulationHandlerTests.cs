using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Infrastructure.Cars;
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
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_CarShouldPassTheIntersectionAsExpected()
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
			ICarGeneratorRepository carGeneratorRepository = new CarGe
			ICarRepository carRepository = new CarsRepositoryInMemory();

			ISimulationHandler simulationHandler =
				new IntersectionSimulationHandler(carRepository);

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			ErrorOr<SimulationState> state;

			do
			{
				state = await simulationHandler.GetState();

				state.IsError.Should().BeFalse();


			} while (state.Value.SimulationPhase == SimulationPhase.InProgress);

			// TODO: Print final results
		}
	}
}