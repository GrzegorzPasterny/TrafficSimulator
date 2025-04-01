using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Reflection;
using TrafficSimulator.Application.Cars.CommitCarsMovement;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.TrafficLights.Handlers.Dynamic;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.CarGenerators.DomainEvents;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.Traffic
{
    public class SimpleDynamicTrafficLightsHandlerTests
	{
		internal readonly ILogger<SimpleDynamicTrafficLightsHandlerTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;
		internal readonly IMediator _mediator;
		internal readonly SimpleDynamicTrafficLightsHandler _handler;

		public SimpleDynamicTrafficLightsHandlerTests(ITestOutputHelper testOutputHelper)
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

			_logger = _loggerFactory.CreateLogger<SimpleDynamicTrafficLightsHandlerTests>();

			_handler = provider.GetRequiredService<SimpleDynamicTrafficLightsHandler>();
			_handler.MinimalTimeForOnePhase = TimeSpan.FromSeconds(1.5);
		}

		[Fact]
		public async Task SetLights_ShouldChooseTheCorrectTrafficPhase()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;
			Intersection intersection = intersectionSimulation.Intersection;

			InboundLane eastInboundLane = intersection.LanesCollection
				.Single(l => l.WorldDirection == WorldDirection.East)
				.InboundLanes[0];
			InboundLane westInboundLane = intersection.LanesCollection
				.Single(l => l.WorldDirection == WorldDirection.West)
				.InboundLanes[0];

			TrafficPhasesHandler trafficPhasesHandler = new TrafficPhasesHandler();
			trafficPhasesHandler.LoadIntersection(intersection);

			var isCarWaitingProperty = typeof(Car).GetProperty("IsCarWaiting",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			for (int i = 0; i < 2; i++)
			{
				var car = new Car(eastInboundLane);
				isCarWaitingProperty!.SetValue(car, true);
				await _mediator.Send(new AddCarCommandDomainEvent(car));
			}

			for (int i = 0; i < 1; i++)
			{
				var car = new Car(westInboundLane);
				isCarWaitingProperty!.SetValue(car, true);
				await _mediator.Send(new AddCarCommandDomainEvent(car));
			}
			await _mediator.Send(new CommitCarsMovementCommand());

			_handler.LoadIntersection(intersection);
			TrafficPhase initialTrafficPhase = _handler.GetCurrentTrafficPhase();

			// Act
			_ = await _handler.SetLights(TimeSpan.FromSeconds(1));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo(initialTrafficPhase.Name);

			_ = await _handler.SetLights(TimeSpan.FromSeconds(1));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForEastOnly");

			for (int i = 0; i < 2; i++)
			{
				var car = new Car(westInboundLane);
				isCarWaitingProperty.SetValue(car, true);
				await _mediator.Send(new AddCarCommandDomainEvent(car));
			}
			await _mediator.Send(new CommitCarsMovementCommand());

			_ = await _handler.SetLights(TimeSpan.FromSeconds(2));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForWestOnly");
		}
	}
}
