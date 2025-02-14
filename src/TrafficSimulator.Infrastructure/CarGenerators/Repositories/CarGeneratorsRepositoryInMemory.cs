using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Infrastructure.CarGenerators.Repositories
{
	public class CarGeneratorsRepositoryInMemory : ICarGeneratorRepository
	{
		public List<ICarGenerator> CarGenerators { get; set; } = new();

		public Task AddCarGeneratorAsync(ICarGenerator carGenerator)
		{
			CarGenerators.Add(carGenerator);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<ICarGenerator>> GetCarGeneratorsAsync()
		{
			return Task.FromResult<IEnumerable<ICarGenerator>>(CarGenerators);
		}
	}
}
