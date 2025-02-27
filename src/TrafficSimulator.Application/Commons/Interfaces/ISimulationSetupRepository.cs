using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ISimulationSetupRepository
	{
		UnitResult<Error> Save(IntersectionSimulation intersectionSimulation);
		ErrorOr<IntersectionSimulation> Load(Guid id);
		ErrorOr<IntersectionSimulation> Load(string identifier);
		ErrorOr<List<IntersectionSimulation>> LoadAll();
	}
}
