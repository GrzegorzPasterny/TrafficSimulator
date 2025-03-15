using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Simulation
{
	public class SimulationStateEventArgs : EventArgs
	{
		public int SimulationStep { get; }
		public Dictionary<Guid, TrafficLightState> TrafficLightsState { get; } = new();
		public Dictionary<Guid, CarLocation> CarLocations { get; } = new();
		public string CurrentTrafficPhaseName { get; set; }

		public SimulationStateEventArgs(int step)
		{
			SimulationStep = step;
		}
	}
}
