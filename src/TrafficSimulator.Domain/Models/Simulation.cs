using CSharpFunctionalExtensions;
using ErrorOr;
using System.Diagnostics;
using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class Simulation : Entity
	{
		private Stopwatch? _simulationStopwatch;
		private readonly Intersection _intersection;
		public SimulationPhase SimulationState { get; private set; }

		// TODO: Load options through configuration
		public SimulationOptions Options { get; set; } = new();

		public Simulation(Intersection intersection)
		{
			_intersection = intersection;
		}

		public UnitResult<Error> Start()
		{
			if (SimulationState != SimulationPhase.NotStarted)
			{
				return DomainErrors.SimulationStateChange(SimulationState, SimulationPhase.InProgress);
			}

			_simulationStopwatch = Stopwatch.StartNew();
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> Abort()
		{
			if (SimulationState != SimulationPhase.InProgress)
			{
				return DomainErrors.SimulationStateChange(SimulationState, SimulationPhase.Finished);
			}

			_simulationStopwatch?.Stop();
			return UnitResult.Success<Error>();
		}
	}
}
