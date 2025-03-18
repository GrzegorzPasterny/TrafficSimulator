using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Lights.HandlerTypes;

namespace TrafficSimulator.Application.TrafficLights.Handlers
{
	public class NullTrafficLightsHandlerFactory : ITrafficLightsHandlerFactory
	{
		public ITrafficLightsHandler CreateHandler(string mode = "Sequential")
		{
			return new NullTrafficLightsHandler();
		}
	}
}
