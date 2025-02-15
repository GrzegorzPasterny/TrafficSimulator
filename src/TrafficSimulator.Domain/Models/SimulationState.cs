using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Domain.Models
{
	public class SimulationState
	{
		public SimulationPhase SimulationPhase { set; get; }

		public List<Car> Cars { get; set; }
	}
}
