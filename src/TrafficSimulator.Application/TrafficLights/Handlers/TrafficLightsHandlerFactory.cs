using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.TrafficLights.Handlers;

namespace TrafficSimulator.Application.Lights.HandlerTypes
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
				_ => throw new ArgumentException($"Invalid Traffic Lights Handler mode: {mode}", nameof(mode))
			};
		}
	}
}
