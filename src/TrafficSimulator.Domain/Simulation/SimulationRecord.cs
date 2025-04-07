using TrafficSimulator.Domain.Simulation.Snapshots;

namespace TrafficSimulator.Domain.Simulation
{
	public class SimulationRecord
	{
		public SimulationRecord(DateTime startTime, Guid simulationId, string simulationName)
		{
			StartTime = startTime;
			SimulationId = simulationId;
			SimulationName = simulationName;
		}

		public List<IntersectionSnapshot> SimulationSnapshots { get; } = [];

		public readonly DateTime StartTime;
		public readonly Guid SimulationId;
		public readonly string SimulationName;
	}
}
