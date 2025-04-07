using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Simulation.Snapshots
{
	public class TrafficLightsSnapshot
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public TrafficLightState TrafficLightState { get; set; }
	}
}
