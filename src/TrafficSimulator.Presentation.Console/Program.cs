using ConsoleAppFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TrafficSimulator.Application;
using TrafficSimulator.Domain;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Presentation.Console.Controllers;

try
{
	var configuration = new ConfigurationBuilder()
		.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		.Build();

	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(configuration)
		.CreateLogger();

	var app = ConsoleApp.Create()
				.ConfigureEmptyConfiguration(builder =>
				{
					builder.AddConfiguration(configuration);
				})
				.ConfigureLogging(builder =>
				{
					builder.AddSerilog();
				})
				.ConfigureServices((context, services) =>
				{
					// Bind appsettings.json to AppSettings class
					//services.Configure<AppSettings>(context.Configuration);
					services.AddScoped<IntersectionSimulationController>();

					// Register application service
					services.AddDomain();
					services.AddApplication();
					services.AddInfrastructure();
				});

	app.Add<IntersectionSimulationController>();

	Log.Information("TrafficSimulator application is starting");

	app.Run(args);
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}

