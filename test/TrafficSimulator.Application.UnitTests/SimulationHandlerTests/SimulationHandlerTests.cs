using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.CarGenerators.Generators;
using TrafficSimulator.Infrastructure.CarGenerators.Repositories;
using TrafficSimulator.Infrastructure.Cars;
using TrafficSimulator.Infrastructure.Intersections;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.SimulationHandlerTests
{
	public class SimulationHandlerTests
	{
		private readonly ILogger<SimulationHandlerTests> _logger;
		private readonly ILoggerFactory _loggerFactory;

		public SimulationHandlerTests(ITestOutputHelper testOutputHelper)
		{
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

			inboundLane.CarGenerator = new SingleCarGenerator(intersection, inboundLane, null);

			IIntersectionRepository intersectionRepository = new IntersectionManager();
			ICarGeneratorRepository carGeneratorRepository = new CarGeneratorsRepositoryInMemory();
			ICarRepository carRepository = new CarsRepositoryInMemory();

			ISimulationHandler simulationHandler =
				new IntersectionSimulationHandler(carGeneratorRepository, carRepository);

			simulationHandler.LoadIntersection(intersection).IsSuccess.Should().BeTrue();

			simulationHandler.Start();

			ErrorOr<SimulationState> state;

			do
			{
				state = await simulationHandler.GetState();

				state.IsError.Should().BeFalse();
				state.Value.SimulationPhase.Should().NotBe(SimulationPhase.NotStarted);

				_logger.LogInformation("Next cycle");

			} while (state.Value.SimulationPhase == SimulationPhase.InProgress);



			// TODO: Print final results
		}
	}
}