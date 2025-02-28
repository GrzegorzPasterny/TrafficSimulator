using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.CarGenerators;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Simulation;

namespace TrafficSimulator.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddMediatR(options =>
			{
				options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
			});

			services.AddScoped<TrafficPhasesHandler>();
			services.AddSingleton<CarGeneratorFactory>();
			services.AddScoped<ITrafficLightsHandler, SimpleSequentialTrafficLightsHandler>();
			services.AddScoped<ISimulationHandler, InMemoryIntersectionSimulationHandler>();

			return services;
		}
	}
}
