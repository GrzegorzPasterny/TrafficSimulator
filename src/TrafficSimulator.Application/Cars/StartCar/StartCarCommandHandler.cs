using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.StartCar
{
	public class StartCarCommandHandler : INotificationHandler<StartCarCommand>
	{
		private readonly ICarRepository _carRepository;

		public StartCarCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public Task Handle(StartCarCommand notification, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
