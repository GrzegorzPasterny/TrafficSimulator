using TrafficSimulator.Domain.Cars;

namespace TrafficSimulator.Application.Commons.Interfaces
{
    public interface ICarRepository
	{
		Task AddCarAsync(Car car);

		Task<IEnumerable<Car>> GetCarsAsync();
	}
}
