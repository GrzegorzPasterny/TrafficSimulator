using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.IntersectionObjects;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler : IDisposable
{
	SimulationState SimulationState { get; }
	SimulationResults SimulationResults { get; }

	UnitResult<Error> LoadIntersection(Intersection intersection);

	Task<UnitResult<Error>> Start();

	UnitResult<Error> Abort();

}

