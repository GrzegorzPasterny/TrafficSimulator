using CSharpFunctionalExtensions;
using System.Diagnostics;

namespace TrafficSimulator.Domain.Models
{
    public class IntersectionSimulation : Entity
	{
		private Intersection _intersection;

		private Stopwatch? _simulationStopwatch;

		// TODO: Load options through configuration
		public SimulationOptions Options { get; set; } = new();

		public IntersectionSimulation(Intersection intersection)
		{
			_intersection = intersection;
		}
	}
}
