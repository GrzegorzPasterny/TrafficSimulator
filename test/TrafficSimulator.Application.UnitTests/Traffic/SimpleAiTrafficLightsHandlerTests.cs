using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.TrafficLights.Handlers.AI;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.Traffic
{
	public class SimpleAiTrafficLightsHandlerTests
	{
		internal readonly ILogger<SimpleAiTrafficLightsHandlerTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;
		internal readonly IMediator _mediator;
		internal readonly SimpleAiTrafficLightsHandler _handler;
		private Mock<IAiAgent> _aiAgent = new();
		private List<float> _aiModelOutput = [0, 0, 0, 0];

		public SimpleAiTrafficLightsHandlerTests(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			var services = new ServiceCollection();
			services.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.AddSerilog(logger, dispose: true);
			});
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();

			var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IAiAgent));
			if (descriptor != null)
			{
				services.Remove(descriptor);
			}

			_aiAgent.Setup(a => a.Predict(It.IsAny<IEnumerable<float>>()))
					.Returns(() => _aiModelOutput.AsReadOnly());
			services.AddSingleton(_aiAgent.Object);

			var provider = services.BuildServiceProvider();
			_mediator = provider.GetRequiredService<IMediator>();

			// Create ILoggerFactory using Serilog
			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog(logger, dispose: true);
			});

			_logger = _loggerFactory.CreateLogger<SimpleAiTrafficLightsHandlerTests>();

			_handler = provider.GetRequiredService<SimpleAiTrafficLightsHandler>();
			_handler.MinimalTimeForOnePhase = TimeSpan.FromSeconds(1.5);
		}

		[Fact]
		public async Task SetLights_ForIntersectionWithOneInboundAndOutboundLaneOnEachDirection_ShouldChooseTheCorrectTrafficPhase()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = IntersectionsRepository.FourDirectional_Full(null);
			Intersection intersection = intersectionSimulation.Intersection;

			_handler.MinimalTimeForOnePhase = TimeSpan.FromSeconds(1.5);

			_handler.LoadIntersection(intersection);
			TrafficPhase initialTrafficPhase = _handler.GetCurrentTrafficPhase();

			// Act
			_ = await _handler.SetLights(TimeSpan.FromSeconds(1));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo(initialTrafficPhase.Name);

			_aiModelOutput[1] = 1;
			_ = await _handler.SetLights(TimeSpan.FromSeconds(1));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForEastOnly");

			_aiModelOutput[3] = 2;
			_ = await _handler.SetLights(TimeSpan.FromSeconds(2));

			_handler.GetCurrentTrafficPhase().Name.Should().BeEquivalentTo("GreenForWestOnly");
		}
	}
}
