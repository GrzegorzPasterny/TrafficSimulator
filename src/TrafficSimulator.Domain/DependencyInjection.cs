using Microsoft.Extensions.DependencyInjection;

namespace TrafficSimulator.Domain
{
	public static class DependencyInjection
	{
		public static ServiceCollection AddDomain(this ServiceCollection services)
		{
			services.AddMediatR(options =>
			{
				options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
			});

			return services;
		}
	}
}
