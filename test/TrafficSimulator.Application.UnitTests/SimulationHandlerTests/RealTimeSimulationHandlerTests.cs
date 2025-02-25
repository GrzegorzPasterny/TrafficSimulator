using FluentAssertions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.CarGenerators;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class RealTimeSimulationHandlerTests : SimulationHandlerTestsBase
	{
		public RealTimeSimulationHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Theory]
		[InlineData("AllGreen", SimulationPhase.Finished)]
		[InlineData("AllRed", SimulationPhase.Aborted)]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_GivenLightsCondition_CarShouldPassTheIntersectionAsExpected(
			string trafficLightsPhaseName,
			SimulationPhase expectedSimulationPhase)
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

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);
			trafficPhasesHandler.SetPhase(trafficLightsPhaseName);

			using ISimulationHandler simulationHandler =
				new RealTimeIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, new NullTrafficLightsHandler(), _loggerFactory.CreateLogger<RealTimeIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			await simulationHandler.Start();

			// track the progress in real time
			do
			{
				await Task.Delay(100);

				simulationHandler.SimulationState.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (simulationHandler.SimulationState.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			simulationHandler.SimulationState.SimulationPhase.Should().Be(expectedSimulationPhase);

			await Task.Delay(1000);
			_logger.LogInformation("SimulationResults = {SimulationResults}", simulationHandler.SimulationResults);
		}

		[Fact]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
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

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleSequentialTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new RealTimeIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, trafficLightsHandler, _loggerFactory.CreateLogger<RealTimeIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			await simulationHandler.Start();

			// track the progress in real time
			do
			{
				await Task.Delay(100);

				simulationHandler.SimulationState.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (simulationHandler.SimulationState.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(1000);
			_logger.LogInformation("SimulationResults = {SimulationResults}", simulationHandler.SimulationResults);
		}
	}
}
