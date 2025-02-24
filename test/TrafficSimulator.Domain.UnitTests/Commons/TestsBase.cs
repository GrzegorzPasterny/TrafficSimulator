using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.Commons
{
	[ExcludeFromCodeCoverage]
	public class TestsBase
	{
		internal readonly ILogger<TestsBase> _logger;
		internal readonly ILoggerFactory _loggerFactory;

		public TestsBase(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog(logger, dispose: true);
			});

			_logger = _loggerFactory.CreateLogger<TestsBase>();
		}
	}
}
