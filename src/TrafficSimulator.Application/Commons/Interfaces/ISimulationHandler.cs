using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler
{
	SimulationState SimulationState { get; }
	SimulationResults SimulationResults { get; }

	UnitResult<Error> LoadIntersection(Intersection intersection);

	UnitResult<Error> Start();

	UnitResult<Error> Abort();

}

