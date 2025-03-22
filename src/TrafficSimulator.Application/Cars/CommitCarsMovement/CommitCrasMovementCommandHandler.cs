using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.CommitCarsMovement
{
	public class CommitCrasMovementCommandHandler : IRequestHandler<CommitCarsMovementCommand>
	{
		private readonly ICarRepository _carRepository;

		public CommitCrasMovementCommandHandler(ICarRepository carRepository)
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
