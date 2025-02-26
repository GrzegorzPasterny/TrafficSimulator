using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler : IDisposable
{
	SimulationState SimulationState { get; }
	SimulationResults SimulationResults { get; }

	UnitResult<Error> LoadIntersection(IntersectionSimulation intersectionSimulation);

	Task<UnitResult<Error>> Start();

	UnitResult<Error> Abort();

}

