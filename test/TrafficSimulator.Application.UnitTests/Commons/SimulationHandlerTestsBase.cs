using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.UnitTests.SimulationHandlerTests;
using TrafficSimulator.Domain;
using TrafficSimulator.Infrastructure;
using Xunit.Abstractions;

namespace TrafficSimulator.Application.UnitTests.Commons
{
	public abstract class SimulationHandlerTestsBase
	{
		internal readonly ILogger<InMemorySimulationHandlerTests> _logger;
		internal readonly ILoggerFactory _loggerFactory;
		internal readonly IMediator _mediator;
		internal readonly ICarGeneratorRepository _carGeneratorRepository;
		internal readonly ICarRepository _carRepository;

		public SimulationHandlerTestsBase(ITestOutputHelper testOutputHelper)
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

			_carGeneratorRepository = provider.GetRequiredService<ICarGeneratorRepository>();
			_carRepository = provider.GetRequiredService<ICarRepository>();
		}
	}
}
