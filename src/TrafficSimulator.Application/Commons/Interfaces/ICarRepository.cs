using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ICarRepository
	{
		Task AddCarAsync(Car car);

		Task<IEnumerable<Car>> GetCarsAsync();
	}
}
