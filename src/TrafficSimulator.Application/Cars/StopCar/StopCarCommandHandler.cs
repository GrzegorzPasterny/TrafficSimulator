using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Cars.StopCar
{
	public class StopCarCommandHandler : INotificationHandler<StopCarCommand>
	{
		private readonly ICarRepository _carRepository;

		public StopCarCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		public Task Handle(StopCarCommand notification, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
