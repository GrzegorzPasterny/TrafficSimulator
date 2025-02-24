using Microsoft.Extensions.Logging;
using Serilog;

namespace TrafficSimulator.Domain.UnitTests.Commons
{
	public class TestFixture
	{
		internal readonly ILoggerFactory _loggerFactory;

		public TestFixture()
		{
			_loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSerilog();
			});
		}
	}
}
