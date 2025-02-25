using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Simulation
{
	public class IntersectionSimulation : Entity
	{
		public Intersection Intersection { get; }
		public string Name { get; set; } = "IS";

		// TODO: Load options through configuration
		public IntersectionSimulationOptions Options { get; set; } = new();

		public SimulationState SimulationState { get; set; } = new SimulationState();
		public SimulationResults? SimulationResults { get; set; }

		public IntersectionSimulation(Intersection intersection)
		{
			Intersection = intersection;
		}
	}
}
