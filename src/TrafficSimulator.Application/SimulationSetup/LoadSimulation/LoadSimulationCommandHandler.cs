using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.SimulationSetup.LoadSimulation
{
	public class LoadSimulationCommandHandler : IRequestHandler<LoadSimulationCommand, ErrorOr<IntersectionSimulation>>
	{
		private readonly ISimulationSetupRepository _simulationSetupRepository;

		public LoadSimulationCommandHandler(ISimulationSetupRepository simulationSetupRepository)
		{
			_simulationSetupRepository = simulationSetupRepository;
		}

		public Task<ErrorOr<IntersectionSimulation>> Handle(LoadSimulationCommand request, CancellationToken cancellationToken)
		{
			return Task.FromResult(_simulationSetupRepository.Load(request.Identifier));
		}
	}
}
