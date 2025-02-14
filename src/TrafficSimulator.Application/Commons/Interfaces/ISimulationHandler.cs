using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler
{
	UnitResult<Error> LoadIntersection(Intersection intersection);

	UnitResult<Error> Start();

	Task<ErrorOr<SimulationState>> GetState();

	UnitResult<Error> Abort();

}

