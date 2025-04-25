using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using TrafficSimulator.Application;
using TrafficSimulator.Domain;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Presentation.WPF.Helpers;
using TrafficSimulator.Presentation.WPF.ViewModels;
using TrafficSimulator.Presentation.WPF.ViewModels.SimulationElements;
using TrafficSimulator.Presentation.WPF.Views;

namespace TrafficSimulator.Presentation.WPF;

public class Bootstrapper
{
	public IServiceProvider ServiceProvider { get; private set; }

	public Bootstrapper()
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

		InMemorySerilogSink inMemorySink = new();

		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.WriteTo.Sink(inMemorySink)
			.CreateLogger();

		var services = new ServiceCollection();

		services.AddSingleton<IConfiguration>(configuration);
		services.AddSingleton(inMemorySink);

		// Register logging
		services.AddLogging(builder =>
		{
			builder.AddSerilog();
		});

		// Register application layers
		services.AddDomain();
		services.AddApplication();
		services.AddInfrastructure();

		services.AddOptions<SimulationOptions>()
			.BindConfiguration(SimulationOptions.DefaultSectionName);

		// Register ViewModels
		services.AddSingleton<MainViewModel>();

		// Register Views
		services.AddSingleton<MainWindow>();

		// Build Service Provider
		ServiceProvider = services.BuildServiceProvider();
	}
}
