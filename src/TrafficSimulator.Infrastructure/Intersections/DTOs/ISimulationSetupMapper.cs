using ErrorOr;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public interface ISimulationSetupMapper
	{
		ErrorOr<IntersectionSimulationDto> ToDto(Domain.Simulation.IntersectionSimulation intersectionSimulation);
		ErrorOr<Domain.Simulation.IntersectionSimulation> ToDomain(IntersectionSimulationDto intersectionSimulationDto);
	}
}
