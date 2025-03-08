using Serilog.Core;
using Serilog.Events;
using System.Windows.Controls;

namespace TrafficSimulator.Presentation.WPF.Helpers
{
	public class DataGridSink : ILogEventSink
	{
		private readonly DataGrid _dataGrid;

		public DataGridSink(DataGrid dataGrid)
		{
			_dataGrid = dataGrid ?? throw new ArgumentNullException(nameof(dataGrid));
		}

		public void Emit(LogEvent logEvent)
		{
			var logEntry = new
			{
				Timestamp = logEvent.Timestamp.ToString("HH:mm:ss.fff"),
				Level = logEvent.Level.ToString(),
				Message = logEvent.RenderMessage()
			};

			if (_dataGrid.Dispatcher.CheckAccess())
			{
				_dataGrid.Items.Add(logEntry);
				_dataGrid.ScrollIntoView(logEntry);
			}
			else
			{
				_dataGrid.Dispatcher.Invoke(() =>
				{
					_dataGrid.Items.Add(logEntry);
					_dataGrid.ScrollIntoView(logEntry);
				});
			}
		}
	}

}
