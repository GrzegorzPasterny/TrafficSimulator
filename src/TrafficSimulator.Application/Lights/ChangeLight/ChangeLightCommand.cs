using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.ChangeLight
{
	public record ChangeLightCommand(long trafficLightId, TrafficLightState trafficLightStateToBeSet);
}
