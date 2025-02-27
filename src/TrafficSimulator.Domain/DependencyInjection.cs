using Microsoft.Extensions.DependencyInjection;

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

			return services;
		}
	}
}
