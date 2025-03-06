using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.DeleteCars
{
	public class DeleteCarsCommandHandler : IRequestHandler<DeleteCarsCommand>
	{
		private readonly ICarRepository _carRepository;

		public DeleteCarsCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public async Task Handle(DeleteCarsCommand request, CancellationToken cancellationToken)
		{
			await _carRepository.DeleteAll();
		}
	}
}
