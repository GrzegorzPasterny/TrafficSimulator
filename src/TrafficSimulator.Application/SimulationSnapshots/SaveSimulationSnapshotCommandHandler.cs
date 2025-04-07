using MediatR;

namespace TrafficSimulator.Application.SimulationSnapshots
{
	public class SaveSimulationSnapshotCommandHandler : IRequestHandler<SaveSimulationSnapshotCommand>
	{
		public SaveSimulationSnapshotCommandHandler()
		{

		}

		public Task Handle(SaveSimulationSnapshotCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
