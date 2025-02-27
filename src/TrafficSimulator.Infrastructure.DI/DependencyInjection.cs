using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Infrastructure.CarGenerators.Repositories;
using TrafficSimulator.Infrastructure.Cars;

namespace TrafficSimulator.Infrastructure.DI
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddSingleton<ICarGeneratorRepository, CarGeneratorsRepositoryInMemory>();
			services.AddSingleton<ICarRepository, CarsRepositoryInMemory>();

			return services;
		}
	}
}
