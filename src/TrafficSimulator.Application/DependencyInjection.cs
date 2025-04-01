using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers.Dynamic;
using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Application.TrafficLights.Handlers.Manual;
using TrafficSimulator.Application.TrafficLights.Handlers.Sequential;

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
			services.AddScoped<ManualTrafficLightsHandler>();
			services.AddScoped<SimpleSequentialTrafficLightsHandler>();
			services.AddScoped<SimpleDynamicTrafficLightsHandler>();
			services.AddSingleton<ITrafficLightsHandlerFactory, TrafficLightsHandlerFactory>();

			services.AddScoped<RealTimeIntersectionSimulationHandler>();
			// In memory is the default simulation handler
			services.AddScoped<InMemoryIntersectionSimulationHandler>();
			services.AddSingleton<IntersectionSimulationHandlerFactory>();

			return services;
		}
	}
}
