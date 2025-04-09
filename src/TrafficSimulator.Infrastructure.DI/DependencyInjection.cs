using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Infrastructure.AI;
using TrafficSimulator.Infrastructure.Cars;
using TrafficSimulator.Infrastructure.DTOs;
using TrafficSimulator.Infrastructure.IntersectionSimulations;
using TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence;

namespace TrafficSimulator.Infrastructure.DI
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddSingleton<IAiAgent, AiAgent>((sp) => new AiAgent("ML/current.onnx"));
			services.AddSingleton<IAiLearningAgent, AiLearningAgent>(() => new AiLearningAgent(""));
			services.AddSingleton<ICarRepository, CarsRepositoryInMemory>();
			services.AddScoped<ISimulationSetupMapper, IntersectionSimulationDtoMapper>();
			services.AddScoped<ISimulationSetupRepository, JsonSimulationSetupRepository>();

			return services;
		}
	}
}
