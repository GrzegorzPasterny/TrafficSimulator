using ErrorOr;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Domain.Commons
{
	public static class DomainErrors
	{
		public static Error TrafficLightsOff(long id)
			=> Error.Failure("TrafficSimulator.LightsOff",
				$"Not possible to change Traffic Lights state when they are off [TrafficLightsId = {id}]");

		public static Error TrafficLightsAlreadyInRequestedState(long id, TrafficLightState trafficLightState)
			=> Error.Failure("TrafficSimulator.LightsAlreadyInRequestedState",
				$"Traffic lights already in requested state [TrafficLightsId = {id}, TrafficLightsState = {trafficLightState}]");

		public static Error TrafficLightsOverallState(long id, bool isOn)
			=> Error.Failure("TrafficSimulator.LightsAlreadyInRequestedOverallState",
				$"Traffic lights already in requested state [TrafficLightsId = {id}, TrafficLightsState = {isOn}]");
	}
}
