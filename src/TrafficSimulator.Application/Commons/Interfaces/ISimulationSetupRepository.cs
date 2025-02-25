using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.Simulation;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ISimulationSetupRepository
	{
		UnitResult<Error> Save(IntersectionSimulation intersectionSimulation);
		ErrorOr<IntersectionSimulation> Load();
	}
}
