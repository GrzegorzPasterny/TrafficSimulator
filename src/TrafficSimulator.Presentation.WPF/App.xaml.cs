using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TrafficSimulator.Presentation.WPF.Views;

namespace TrafficSimulator.Presentation.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{
		private readonly Bootstrapper _bootstrapper;

		public App()
		{
			_bootstrapper = new Bootstrapper();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var mainWindow = _bootstrapper.ServiceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
		}
	}
}
