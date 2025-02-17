using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Infrastructure.CarGenerators.Repositories;
using TrafficSimulator.Infrastructure.Cars;

namespace TrafficSimulator.Infrastructure
{
	public static class DependencyInjection
	{
		public static ServiceCollection AddInfrastructure(this ServiceCollection services)
		{
			services.AddSingleton<ICarGeneratorRepository, CarGeneratorsRepositoryInMemory>();
			services.AddSingleton<ICarRepository, CarsRepositoryInMemory>();

			return services;
		}
	}
}
