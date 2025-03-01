using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.CarGenerators.DomainEvents;

namespace TrafficSimulator.Application.Cars.AddCar
{
	public class AddCarCommandHandler : IRequestHandler<AddCarCommandDomainEvent>
	{
		private readonly ICarRepository _carRepository;

		public AddCarCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public async Task Handle(AddCarCommandDomainEvent command, CancellationToken cancellationToken)
		{
			await _carRepository.AddCarAsync(command.Car);
		}
	}
}
