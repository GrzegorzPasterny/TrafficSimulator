using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class SimulationHandlerTests
	{
		private readonly ILogger<SimulationHandlerTests> _logger;
		private readonly ILoggerFactory _loggerFactory;
		private readonly IMediator _mediator;
		private readonly ICarGeneratorRepository _carGeneratorRepository;
		private readonly ICarRepository _carRepository;

		public SimulationHandlerTests(ITestOutputHelper testOutputHelper)
		{
			var services = new ServiceCollection();
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();
			var provider = services.BuildServiceProvider();
			_mediator = provider.GetRequiredService<IMediator>();

			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose)
				.CreateLogger();

			// Create ILoggerFactory using Serilog
			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog(logger, dispose: true);
			});

			_logger = _loggerFactory.CreateLogger<SimulationHandlerTests>();

			_carGeneratorRepository = provider.GetRequiredService<ICarGeneratorRepository>();
			_carRepository = provider.GetRequiredService<ICarRepository>();
		}

		[Fact]
		public async Task RunSimulation_GivenSimpleIntersection_GivenOneCar_CarShouldPassTheIntersectionAsExpected()
		{
			// Arrange
			ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create()
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddLane(WorldDirection.East, true)
				//.AddCarGenerator()
				.AddLane(WorldDirection.East, false)
				.AddLanesCollection(WorldDirection.West)
				.AddLane(WorldDirection.West, true)
				.AddLane(WorldDirection.West, false)
				.Build();

			intersectionResult.IsError.Should().BeFalse();

			Intersection intersection = intersectionResult.Value;

			InboundLane inboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator carGenerator = new SingleCarGenerator(intersection, inboundLane, _mediator);

			inboundLane.CarGenerator = carGenerator;
			await _carGeneratorRepository.AddCarGeneratorAsync(carGenerator);

			ISimulationHandler simulationHandler =
				new IntersectionSimulationHandler(_carGeneratorRepository, _carRepository, _loggerFactory.CreateLogger<IntersectionSimulationHandler>());

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			ErrorOr<SimulationState> state;

			do
			{
				await Task.Delay(100);

				state = await simulationHandler.GetState();

				state.IsError.Should().BeFalse();
				state.Value.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

			} while (state.Value.SimulationPhase is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished);

			// print final result
			state = await simulationHandler.GetState();
		}
	}
}