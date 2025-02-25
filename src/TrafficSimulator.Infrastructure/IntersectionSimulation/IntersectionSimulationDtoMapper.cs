using ErrorOr;
using TrafficSimulator.Infrastructure.DTOs;

namespace TrafficSimulator.Infrastructure.IntersectionSimulation
{
	public class IntersectionSimulationDtoMapper : ISimulationSetupMapper
	{
		public ErrorOr<Domain.Models.Simulation.IntersectionSimulation> ToDomain(IntersectionSimulationDto intersectionSimulationDto)
		{
			throw new NotImplementedException();
		}

		public ErrorOr<IntersectionSimulationDto> ToDto(Domain.Models.Simulation.IntersectionSimulation intersectionSimulation)
		{
			throw new NotImplementedException();
		}
	}
}
