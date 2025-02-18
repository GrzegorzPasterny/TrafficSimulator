using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class InMemorySimulationHandlerTests : SimulationHandlerTestsBase
	{
		public InMemorySimulationHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
				new InMemoryIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			// It takes few hundret milliseconds for simulation to finish
			await Task.Delay(3000);

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
			_logger.LogInformation("SimulationResults = {SimulationResults}", simulationHandler.SimulationResults);
		}
	}
}