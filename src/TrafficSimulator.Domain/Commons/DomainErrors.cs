using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.Lights;

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

		public static Error SimulationStateChange(SimulationPhase currentSimulationState, SimulationPhase requestedSimulationState)
			=> Error.Failure("TrafficSimulator.SimulationStateChange",
				$"Invalid attempt to change simulation state " +
				$"[CurrentSimulationState = {currentSimulationState}, RequestedSimulationState = {requestedSimulationState}]");

		public static Error CarHasReachedDestination(long id)
			=> Error.Failure("TrafficSimulator.CarHasReachedDestination",
				$"Not possible for car to move when it reached its destination [CarId = {id}]");
	}
}
