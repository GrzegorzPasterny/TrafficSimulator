using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Models
{
	public class IntersectionSimulation : Entity
	{
		private Intersection _intersection;

		// TODO: Load options through configuration
		public SimulationOptions Options { get; set; } = new();

		public SimulationState SimulationState { get; set; } = new SimulationState();
		public SimulationResults SimulationResults { get; set; }

		public IntersectionSimulation(Intersection intersection)
		{
			_intersection = intersection;
		}
	}
}
