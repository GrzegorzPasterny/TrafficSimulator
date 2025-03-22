using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ICarRepository
	{
		Task AddCarAsync(Car car);
		void CommitPendingCars();
		Task DeleteAll();
		Task<IEnumerable<Car>> GetCarsAsync();
	}
}
