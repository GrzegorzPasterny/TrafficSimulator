using TrafficSimulator.Domain.IntersectionObjects.IntersectionProperties;

namespace TrafficSimulator.Application.TrafficLights.ChangeLight
{
    public record ChangeLightCommand(long trafficLightId, TrafficLightState trafficLightStateToBeSet);
}
