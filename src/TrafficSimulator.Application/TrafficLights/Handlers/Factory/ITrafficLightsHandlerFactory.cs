using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.TrafficLights.Handlers.Factory
{
    public interface ITrafficLightsHandlerFactory
    {
        ITrafficLightsHandler CreateHandler(string mode = TrafficLightHandlerTypes.Sequential);
    }
}