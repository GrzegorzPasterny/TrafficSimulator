using FluentAssertions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class RealTimeSimulationHandlerTests : SimulationHandlerTestsBase
	{
		public RealTimeSimulationHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			Intersection intersection = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator carGenerator = new SingleCarGenerator(intersection, inboundLane, _mediator);

			inboundLane.CarGenerator = carGenerator;
			await _carGeneratorRepository.AddCarGeneratorAsync(carGenerator);

			ISimulationHandler simulationHandler =
				new RealTimeIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, _loggerFactory.CreateLogger<RealTimeIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			SimulationState state;

			// track the progress in real time
			do
			{
				await Task.Delay(100);

				state = simulationHandler.GetState();

				state.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (state.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			// print the final result
			state = simulationHandler.GetState();

			state.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}
	}
}
