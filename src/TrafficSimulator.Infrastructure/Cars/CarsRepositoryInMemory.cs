using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Infrastructure.Cars
{
	using System.Collections.Concurrent;

	public class CarsRepositoryInMemory : ICarRepository
	{
		private readonly List<Car> _cars = new();
		private readonly ConcurrentQueue<Car> _pendingCars = new(); // Thread-safe queue

		public Task AddCarAsync(Car car)
		{
			_pendingCars.Enqueue(car); // Safe to add cars at any time
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Car>> GetCarsAsync()
		{
			lock (_cars)
			{
				return Task.FromResult(_cars.ToList() as IEnumerable<Car>); // Return a copy
			}
		}

		public void CommitPendingCars()
		{
			while (_pendingCars.TryDequeue(out var car)) // Add queued cars safely
			{
				_cars.Add(car);
			}
		}

		public Task DeleteAll()
		{
			lock (_cars)
			{
				_cars.Clear();
			}
			return Task.CompletedTask;
		}
	}

}
