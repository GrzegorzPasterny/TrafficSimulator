using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.SimulationSetup.LoadSimulation
{
	public record LoadSimulationCommand(string Identifier) : IRequest<ErrorOr<IntersectionSimulation>>;
}
