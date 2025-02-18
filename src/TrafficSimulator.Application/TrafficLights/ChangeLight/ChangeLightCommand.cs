using TrafficSimulator.Domain.Models.TrafficLights;

namespace TrafficSimulator.Application.TrafficLights.ChangeLight
{
    public record ChangeLightCommand(long trafficLightId, TrafficLightState trafficLightStateToBeSet);
}
