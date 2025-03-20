using CSharpFunctionalExtensions;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Simulation
{
	public class IntersectionSimulation : Entity<Guid>
	{
		public Intersection Intersection { get; }
		public string Name { get; }

		public IntersectionSimulationOptions Options { get; set; } = new();

		public SimulationState SimulationState { get; set; } = new SimulationState();
		public SimulationResults? SimulationResults { get; set; }

		public IntersectionSimulation(Intersection intersection, string name = "IS")
		{
			Id = Guid.NewGuid();
			Name = name;
			Intersection = intersection;
		}

		public IntersectionSimulation(Intersection intersection, Guid id, string name = "IS")
		{
			Id = id;
			Name = name;
			Intersection = intersection;
		}

		public void Reset()
		{
			List<ICarGenerator> carGenerators = SimulationState.CarGenerators;
			carGenerators.ForEach(carGenerator => carGenerator.Reset());

			SimulationState = new SimulationState();
			SimulationResults = new SimulationResults();

			SimulationState.CarGenerators = carGenerators;
		}
	}
}
