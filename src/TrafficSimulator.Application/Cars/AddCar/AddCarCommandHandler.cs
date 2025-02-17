using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.AddCar
{
	public class AddCarCommandHandler : IRequestHandler<AddCarCommand>
	{
		private readonly ICarRepository _carRepository;

		public AddCarCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public async Task Handle(AddCarCommand command, CancellationToken cancellationToken)
		{
			await _carRepository.AddCarAsync(command.Car);
		}
	}
}
