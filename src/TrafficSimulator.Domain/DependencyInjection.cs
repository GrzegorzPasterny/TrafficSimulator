using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Domain.CarGenerators;

namespace TrafficSimulator.Domain
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDomain(this IServiceCollection services)
		{
			services.AddMediatR(options =>
			{
				options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
			});

			services.AddSingleton(sp => new CarGeneratorFactory(sp));

			return services;
		}
	}
}
