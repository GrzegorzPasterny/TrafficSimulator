using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace TrafficSimulator.Domain.UnitTests.Commons
{
	[ExcludeFromCodeCoverage]
	public abstract class TestsBase
	{
		internal readonly ILogger<TestsBase> _logger;
		internal readonly ILoggerFactory _loggerFactory;

		public TestsBase(TestFixture testFixture, ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			_loggerFactory = testFixture._loggerFactory;
			_loggerFactory.AddSerilog(logger);

			_logger = _loggerFactory.CreateLogger<TestsBase>();
		}
	}
}
