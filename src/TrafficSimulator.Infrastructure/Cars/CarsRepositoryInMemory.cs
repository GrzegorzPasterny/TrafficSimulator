using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Infrastructure.Cars
{
	public class CarsRepositoryInMemory : ICarRepository
	{
		public List<Car> Cars { get; set; } = new();

		public Task AddCarAsync(Car car)
		{
			Cars.Add(car);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<Car>> GetCarsAsync()
		{
			return Task.FromResult<IEnumerable<Car>>(Cars);
		}
	}
}
