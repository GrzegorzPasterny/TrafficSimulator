using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ISimulationSetupRepository
	{
		UnitResult<Error> Save(Domain.Simulation.IntersectionSimulation intersectionSimulation);
		ErrorOr<Domain.Simulation.IntersectionSimulation> Load();
	}
}
