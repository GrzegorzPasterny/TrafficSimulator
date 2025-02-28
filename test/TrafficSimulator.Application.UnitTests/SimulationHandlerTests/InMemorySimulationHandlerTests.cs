using CSharpFunctionalExtensions;
using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.CarGenerators;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class InMemorySimulationHandlerTests
	{
		internal readonly ILogger<InMemorySimulationHandlerTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;
		internal readonly IMediator _mediator;
		internal readonly ICarRepository _carRepository;

		public InMemorySimulationHandlerTests(ITestOutputHelper testOutputHelper)
		{
			var services = new ServiceCollection();
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();
			var provider = services.BuildServiceProvider();
			_mediator = provider.GetRequiredService<IMediator>();

			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			// Create ILoggerFactory using Serilog
			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog(logger, dispose: true);
			});

			_logger = _loggerFactory.CreateLogger<InMemorySimulationHandlerTests>();

			_carRepository = provider.GetRequiredService<ICarRepository>();
		}

		[Theory]
		[InlineData("AllGreen", SimulationPhase.Finished)]
		[InlineData("AllRed", SimulationPhase.Aborted)]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_GivenLightsCondition_CarShouldPassTheIntersectionAsExpected(
			string trafficLightsPhaseName,
			SimulationPhase expectedSimulationPhase)
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator carGenerator = new SingleCarGenerator(intersection, inboundLane, _mediator);

			inboundLane.CarGenerator = carGenerator;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);
			trafficPhasesHandler.SetPhase(trafficLightsPhaseName);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carRepository, new NullTrafficLightsHandler(), null, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();
			simulationHandler.SimulationState.SimulationPhase.Should().Be(expectedSimulationPhase);
		}

		[Fact]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator carGenerator = new SingleCarGenerator(intersection, inboundLane, _mediator);

			inboundLane.CarGenerator = carGenerator;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleSequentialTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carRepository, trafficLightsHandler, null, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(200); // Wait for the results
		}

		[Fact]
		public async Task RunSimulation_GivenForkIntersection_GivenOneCar_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;
			Intersection intersection = intersectionSimulation.Intersection;

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

			ICarGenerator eastLaneCarGenerator = new SingleCarGenerator(intersection, eastInboundLane, _mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleSequentialTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carRepository, trafficLightsHandler, null, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(200); // Wait for the results
		}

		[Fact]
		public async Task RunSimulation_GivenForkIntersection_GivenMultipleCars_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane westInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, _mediator);
			westInboundLane.CarGenerator = westLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, _mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleSequentialTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carRepository, trafficLightsHandler, null, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(200); // Wait for the results
		}

		[Fact]
		public async Task RunSimulation_GivenThreeDirectionIntersection_GivenMultipleCars_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			InboundLane southInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.South)!
				.InboundLanes!
				.First();

			InboundLane westInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, _mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			ICarGenerator southLaneCarGenerator = new MultipleCarsGenerator(intersection, southInboundLane, _mediator);
			southInboundLane.CarGenerator = southLaneCarGenerator;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, _mediator);
			westInboundLane.CarGenerator = westLaneCarGenerator;

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _loggerFactory.CreateLogger<SimpleSequentialTrafficLightsHandler>());

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_carRepository, trafficLightsHandler, null, _loggerFactory.CreateLogger<InMemoryIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(200); // Wait for the results
		}
	}
}