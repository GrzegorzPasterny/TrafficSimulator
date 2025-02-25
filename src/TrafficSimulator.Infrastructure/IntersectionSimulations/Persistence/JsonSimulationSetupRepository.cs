using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DTOs;

namespace TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence
{
	public class JsonSimulationSetupRepository : ISimulationSetupRepository
	{
		private readonly ISimulationSetupMapper _simulationSetupMapper;

		public JsonSimulationSetupRepository(ISimulationSetupMapper simulationSetupMapper)
		{
			_simulationSetupMapper = simulationSetupMapper;
		}

		public ErrorOr<IntersectionSimulation> Load()
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> Save(IntersectionSimulation intersectionSimulation)
		{
			throw new NotImplementedException();
		}
	}
}
