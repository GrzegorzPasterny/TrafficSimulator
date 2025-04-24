using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.TrafficLights.Handlers.AI;
using TrafficSimulator.Application.TrafficLights.Handlers.Dynamic;
using TrafficSimulator.Application.TrafficLights.Handlers.Manual;
using TrafficSimulator.Application.TrafficLights.Handlers.Nest;
using TrafficSimulator.Application.TrafficLights.Handlers.Sequential;

namespace TrafficSimulator.Application.TrafficLights.Handlers.Factory
{
	public class TrafficLightsHandlerFactory : ITrafficLightsHandlerFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public TrafficLightsHandlerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ITrafficLightsHandler CreateHandler(string mode = TrafficLightHandlerTypes.Sequential)
		{
			return mode switch
			{
				TrafficLightHandlerTypes.Sequential => _serviceProvider.GetRequiredService<SimpleSequentialTrafficLightsHandler>(),
				TrafficLightHandlerTypes.Manual => _serviceProvider.GetRequiredService<ManualTrafficLightsHandler>(),
				TrafficLightHandlerTypes.Dynamic => _serviceProvider.GetRequiredService<SimpleDynamicTrafficLightsHandler>(),
				TrafficLightHandlerTypes.AI => _serviceProvider.GetRequiredService<SimpleAiTrafficLightsHandler>(),
				TrafficLightHandlerTypes.LearningAI => _serviceProvider.GetRequiredService<SimpleAiLearningTrafficLightsHandler>(),
				TrafficLightHandlerTypes.Nest => _serviceProvider.GetRequiredService<NestTrafficLightsHandler>(),
				_ => throw new ArgumentException($"Invalid Traffic Lights Handler mode: {mode}", nameof(mode))
			};
		}
	}
}
