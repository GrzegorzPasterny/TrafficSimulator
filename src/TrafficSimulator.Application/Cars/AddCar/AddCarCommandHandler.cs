using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.AddCar
{
	public class AddCarCommandHandler : INotificationHandler<AddCarCommand>
	{
		private readonly ICarRepository _carRepository;

		public AddCarCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public async Task Handle(AddCarCommand notification, CancellationToken cancellationToken)
		{
			await _carRepository.AddCarAsync(notification.Car);
		}
	}
}
