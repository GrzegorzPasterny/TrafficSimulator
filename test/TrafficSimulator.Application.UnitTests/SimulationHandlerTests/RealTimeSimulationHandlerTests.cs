using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Lights.HandlerTypes;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class RealTimeSimulationHandlerTests
	{
		internal readonly ILogger<RealTimeSimulationHandlerTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;
		internal readonly IMediator _mediator;
		internal readonly ICarRepository _carRepository;
		internal Mock<ITrafficLightsHandlerFactory> _trafficLightsHandlerFactoryMock;

		public RealTimeSimulationHandlerTests(ITestOutputHelper testOutputHelper)
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

			_logger = _loggerFactory.CreateLogger<RealTimeSimulationHandlerTests>();

			_carRepository = provider.GetRequiredService<ICarRepository>();

			_trafficLightsHandlerFactoryMock = new Mock<ITrafficLightsHandlerFactory>();
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
				new RealTimeIntersectionSimulationHandler(_mediator, new NullTrafficLightsHandlerFactory(trafficPhasesHandler.CurrentPhase), _loggerFactory.CreateLogger<RealTimeIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			await simulationHandler.Start();

			// track the progress in real time
			do
			{
				await Task.Delay(100);

				simulationHandler.SimulationState.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (simulationHandler.SimulationState.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			simulationHandler.SimulationState.SimulationPhase.Should().Be(expectedSimulationPhase);

			await Task.Delay(100);
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

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new RealTimeIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _loggerFactory.CreateLogger<RealTimeIntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			await simulationHandler.Start();

			// track the progress in real time
			do
			{
				await Task.Delay(100);

				simulationHandler.SimulationState.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (simulationHandler.SimulationState.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);

			await Task.Delay(100);
		}
	}
}
