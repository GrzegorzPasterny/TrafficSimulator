using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrafficSimulator.Application.SimulationSnapshots
{
	public class SaveSimulationSnapshotCommandHandler : IRequestHandler<SaveSimulationSnapshotCommand>
	{
		private static readonly SemaphoreSlim _fileLock = new(1, 1);

		public async Task Handle(SaveSimulationSnapshotCommand request, CancellationToken cancellationToken)
		{
			string snapshotsDirectory = "Simulation_Snapshots";
			Directory.CreateDirectory(snapshotsDirectory);

			string simulationSnapshotFileName = $"{request.simulationName}_{DateTime.Now:dd.MM.yyyy}_{request.SimulationId}.txt";
			string simulationSnapshotFileFullName = Path.Combine(snapshotsDirectory, simulationSnapshotFileName);

			JsonSerializerOptions options = GetJsonSerializerOptions();
			string snapshotJson = JsonSerializer.Serialize(request, options);

			await _fileLock.WaitAsync(cancellationToken);
			try
			{
				await File.AppendAllTextAsync(simulationSnapshotFileFullName, snapshotJson + Environment.NewLine, cancellationToken);
			}
			finally
			{
				_fileLock.Release();
			}
		}

		private static JsonSerializerOptions GetJsonSerializerOptions()
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true
			};

			options.Converters.Add(new JsonStringEnumConverter());
			return options;
		}
	}
}
