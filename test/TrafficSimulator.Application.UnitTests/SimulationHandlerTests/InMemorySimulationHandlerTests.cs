using CSharpFunctionalExtensions;
using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.Simulation;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class InMemorySimulationHandlerTests : SimulationHandlerTestsBase
	{
		public InMemorySimulationHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.AllLightsGreen(intersection));
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.AllLightsRed(intersection));
			trafficPhasesHandler.SetPhase(trafficLightsPhaseName);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, new NullTrafficLightsHandler(), _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();
			simulationHandler.SimulationState.SimulationPhase.Should().Be(expectedSimulationPhase);
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

			// Default Traffic Lights are Red, that's why They have to be set to Green
			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.AllLightsGreen(intersection));
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.AllLightsRed(intersection));

			ITrafficLightsHandler trafficLightsHandler = new SimpleTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, trafficLightsHandler, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
			_logger.LogInformation("SimulationResults = {SimulationResults}", simulationHandler.SimulationResults);
		}

		[Fact]
		public async Task RunSimulation_GivenForkIntersection_GivenOneCar_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			Intersection intersection = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;

			InboundLane westInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			ICarGenerator westLaneCarGenerator = new SingleCarGenerator(intersection, westInboundLane, _mediator);
			westInboundLane.CarGenerator = westLaneCarGenerator;
			await _carGeneratorRepository.AddCarGeneratorAsync(westLaneCarGenerator);

			ICarGenerator eastLaneCarGenerator = new SingleCarGenerator(intersection, eastInboundLane, _mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;
			await _carGeneratorRepository.AddCarGeneratorAsync(eastLaneCarGenerator);

			// Default Traffic Lights are Red, that's why They have to be set to Green
			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			trafficPhasesHandler.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			ITrafficLightsHandler trafficLightsHandler = new SimpleTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carGeneratorRepository, _carRepository, trafficLightsHandler, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
			_logger.LogInformation("SimulationResults = {SimulationResults}", simulationHandler.SimulationResults);
		}
	}
}