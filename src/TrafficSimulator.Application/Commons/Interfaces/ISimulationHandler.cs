using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler
{
	UnitResult<Error> LoadIntersection(Intersection intersection);

	UnitResult<Error> Start();

	SimulationState GetState();

	UnitResult<Error> Abort();

}

