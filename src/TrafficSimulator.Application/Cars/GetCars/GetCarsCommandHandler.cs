using MediatR;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Cars.GetCars
{
	public class GetCarsCommandHandler : IRequestHandler<GetCarsCommand, IEnumerable<Car>>
	{
		private readonly ICarRepository _carRepository;

		public GetCarsCommandHandler(ICarRepository carRepository)
		{
			_carRepository = carRepository;
		}

		Task<IEnumerable<Car>> IRequestHandler<GetCarsCommand, IEnumerable<Car>>.Handle(GetCarsCommand request, CancellationToken cancellationToken)
		{
			return _carRepository.GetCarsAsync();
		}
	}
}
