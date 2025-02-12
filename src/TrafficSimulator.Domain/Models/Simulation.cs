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
		public SimulationState SimulationState { get; private set; }

		public Simulation(Intersection intersection)
		{
			_intersection = intersection;
		}

		public UnitResult<Error> Start()
		{
			if (SimulationState != SimulationState.NotStarted)
			{
				return DomainErrors.SimulationStateChange(SimulationState, SimulationState.InProgress);
			}

			_simulationStopwatch = Stopwatch.StartNew();
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> Abort()
		{
			if (SimulationState != SimulationState.InProgress)
			{
				return DomainErrors.SimulationStateChange(SimulationState, SimulationState.Finished);
			}

			_simulationStopwatch.Stop();
			return UnitResult.Success<Error>();
		}
	}
}
