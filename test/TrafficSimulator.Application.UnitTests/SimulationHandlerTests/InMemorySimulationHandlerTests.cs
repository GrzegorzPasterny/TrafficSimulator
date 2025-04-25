using CSharpFunctionalExtensions;
using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using SharpNeat;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Application.TrafficLights.Handlers.Sequential;
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
	public class InMemorySimulationHandlerTests
	{
		internal readonly ILogger<InMemorySimulationHandlerTests> _logger;
		internal readonly IMediator _mediator;
		internal readonly IServiceProvider _provider;
		internal Mock<ITrafficLightsHandlerFactory> _trafficLightsHandlerFactoryMock;
		internal Mock<IBlackBox<double>> _blackBoxMock = new Mock<IBlackBox<double>>();

		public InMemorySimulationHandlerTests(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			var services = new ServiceCollection();
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();

			services.AddLogging(loggerBuilder =>
			{
				loggerBuilder.ClearProviders();
				loggerBuilder.AddSerilog(logger);
			});

			_provider = services.BuildServiceProvider();

			_mediator = _provider.GetRequiredService<IMediator>();
			_logger = _provider.GetRequiredService<ILogger<InMemorySimulationHandlerTests>>();

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

			TrafficPhasesHandler trafficPhasesHandler = new();
			trafficPhasesHandler.LoadIntersection(intersection);
			trafficPhasesHandler.SetPhase(trafficLightsPhaseName, TimeSpan.FromSeconds(1));

			ITrafficLightsHandler trafficLightsHandler = new NullTrafficLightsHandler(trafficPhasesHandler.CurrentPhase!);

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _provider.GetRequiredService<ILogger<InMemoryIntersectionSimulationHandler>>());

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

			TrafficPhasesHandler trafficPhasesHandler = new();
			trafficPhasesHandler.LoadIntersection(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _provider.GetRequiredService<ILogger<SimpleSequentialTrafficLightsHandler>>());

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _provider.GetRequiredService<ILogger<InMemoryIntersectionSimulationHandler>>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}

		[Fact]
		public async Task RunSimulation_GivenForkIntersection_GivenMultipleCars_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLightsWithMultipleCarGenerators(_mediator);
			Intersection intersection = intersectionSimulation.Intersection;

			TrafficPhasesHandler trafficPhasesHandler = new();
			trafficPhasesHandler.LoadIntersection(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _provider.GetRequiredService<ILogger<SimpleSequentialTrafficLightsHandler>>());

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _provider.GetRequiredService<ILogger<InMemoryIntersectionSimulationHandler>>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}

		[Fact]
		public async Task RunSimulation_GivenThreeDirectionIntersection_GivenMultipleCars_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(_mediator);
			Intersection intersection = intersectionSimulation.Intersection;

			TrafficPhasesHandler trafficPhasesHandler = new();
			trafficPhasesHandler.LoadIntersection(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _provider.GetRequiredService<ILogger<SimpleSequentialTrafficLightsHandler>>());

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _provider.GetRequiredService<ILogger<InMemoryIntersectionSimulationHandler>>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}

		[Fact]
		public async Task RunSimulation_GivenFourDirectionIntersection_GivenMultipleCars_GivenSimpleTrafficHandler_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.FourDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(_mediator);
			Intersection intersection = intersectionSimulation.Intersection;

			TrafficPhasesHandler trafficPhasesHandler = new();
			trafficPhasesHandler.LoadIntersection(intersection);

			ITrafficLightsHandler trafficLightsHandler = new SimpleSequentialTrafficLightsHandler(trafficPhasesHandler, _provider.GetRequiredService<ILogger<SimpleSequentialTrafficLightsHandler>>());

			_trafficLightsHandlerFactoryMock.Setup(factory => factory.CreateHandler(It.IsAny<string>()))
				.Returns(() => trafficLightsHandler);

			using ISimulationHandler simulationHandler =
				new InMemoryIntersectionSimulationHandler(_mediator, _trafficLightsHandlerFactoryMock.Object, _provider.GetRequiredService<ILogger<InMemoryIntersectionSimulationHandler>>());

			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}

		[Fact]
		public async Task RunForkSimulation_GivenNestTrafficHandler_ShouldFinishTheSimulationSuccessfully()
		{
			// Arrange
			_blackBoxMock
				.Setup(box => box.Inputs)
				.Returns(new double[9]);

			_blackBoxMock
				.Setup(box => box.Outputs)
				.Returns(() => new Memory<double>([Random.Shared.NextDouble(), Random.Shared.NextDouble()]));

			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLane_NestSimulation(_mediator, _blackBoxMock.Object);
			IntersectionSimulationHandlerFactory intersectionSimulationHandlerFactory = _provider.GetRequiredService<IntersectionSimulationHandlerFactory>();
			ISimulationHandler simulationHandler = intersectionSimulationHandlerFactory.CreateHandler(SimulationMode.InMemory);
			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();

			simulationHandler.SimulationState.SimulationPhase.Should().Be(SimulationPhase.Finished);
		}
	}
}