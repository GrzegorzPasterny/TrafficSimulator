using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.TrafficLights.Handlers;

namespace TrafficSimulator.Application.Lights.HandlerTypes
{
	public interface ITrafficLightsHandlerFactory
	{
		ITrafficLightsHandler CreateHandler(string mode = TrafficLightHandlerTypes.Sequential);
	}
}