using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.CommitCarsMovement
{
	public class CommitCarsMovementCommandHandler : IRequestHandler<CommitCarsMovementCommand>
	{
		private readonly ICarRepository _carRepository;

		public CommitCarsMovementCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public Task Handle(CommitCarsMovementCommand request, CancellationToken cancellationToken)
		{
			_carRepository.CommitPendingCars();

			return Task.CompletedTask;
		}
	}
}
