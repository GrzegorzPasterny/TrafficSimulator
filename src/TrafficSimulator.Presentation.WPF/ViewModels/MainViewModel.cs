using Microsoft.Extensions.Logging;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public class MainViewModel
	{
		private readonly ILogger<MainViewModel> _logger;

		public MainViewModel(ILogger<MainViewModel> logger)
		{
			_logger = logger;
			_logger.LogInformation("MainViewModel initialized");
		}
	}
}
