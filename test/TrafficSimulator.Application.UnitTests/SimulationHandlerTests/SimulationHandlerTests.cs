using ErrorOr;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;
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

				_logger.LogInformation("Next cycle");

			} while (state.Value.SimulationPhase == SimulationPhase.InProgress);

			// TODO: Print final results
		}
	}
}