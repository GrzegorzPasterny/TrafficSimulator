using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System.Collections.Concurrent;
using System.IO;

namespace TrafficSimulator.Presentation.WPF.Helpers
{
	public class InMemorySink : ILogEventSink
	{
		readonly ITextFormatter _textFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{Exception}");

		public ConcurrentQueue<LogEvent> Events { get; } = new();

		public void Emit(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

			var renderSpace = new StringWriter();
			_textFormatter.Format(logEvent, renderSpace);
			Events.Enqueue(logEvent);
		}
	}
}
