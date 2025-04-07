using MediatR;
using TrafficSimulator.Domain.Simulation.Snapshots;

namespace TrafficSimulator.Application.SimulationSnapshots
{
	public record SaveSimulationSnapshotCommand(
		Guid SimulationId,
		string simulationName,
		IntersectionSnapshot IntersectionSnapshot) : IRequest;
}
