using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.Lights.HandlerTypes;
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
			services.AddScoped<ITrafficLightsHandler, ManualTrafficLightsHandler>();
			services.AddScoped<ITrafficLightsHandler, SimpleSequentialTrafficLightsHandler>();
			services.AddSingleton<TrafficLightsHandlerFactory>();

			services.AddScoped<RealTimeIntersectionSimulationHandler>();
			// In memory is the default simulation handler
			services.AddScoped<InMemoryIntersectionSimulationHandler>();
			services.AddSingleton<IntersectionSimulationHandlerFactory>();

			return services;
		}
	}
}
