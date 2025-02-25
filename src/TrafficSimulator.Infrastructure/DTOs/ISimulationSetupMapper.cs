using ErrorOr;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public interface ISimulationSetupMapper
	{
		ErrorOr<IntersectionSimulationDto> ToDto(Domain.Models.Simulation.IntersectionSimulation intersectionSimulation);
		ErrorOr<Domain.Models.Simulation.IntersectionSimulation> ToDomain(IntersectionSimulationDto intersectionSimulationDto);
	}
}
