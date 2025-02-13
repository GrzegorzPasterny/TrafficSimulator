using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.TrafficLights.ChangeLight
{
	public record ChangeLightCommand(long trafficLightId, TrafficLightState trafficLightStateToBeSet);
}
