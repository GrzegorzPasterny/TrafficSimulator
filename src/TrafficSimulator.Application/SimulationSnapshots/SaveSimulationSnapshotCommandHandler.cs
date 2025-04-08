using MediatR;
using System.Text.Json;

namespace TrafficSimulator.Application.SimulationSnapshots
{
	public class SaveSimulationSnapshotCommandHandler : IRequestHandler<SaveSimulationSnapshotCommand>
	{
		public SaveSimulationSnapshotCommandHandler()
		{

		}

		public Task Handle(SaveSimulationSnapshotCommand request, CancellationToken cancellationToken)
		{
			string simulationSnapshotFileName = $"{request.simulationName}_{DateTime.Now:dd.MM.yyyy}_{request.SimulationId}.txt";
			string simulationSnapshotFileFullName = Path.Combine("Simulation_Snapshots", simulationSnapshotFileName);
			string snapshotJson = JsonSerializer.Serialize(request);

			return File.AppendAllTextAsync(simulationSnapshotFileFullName, snapshotJson);
		}
	}
}
