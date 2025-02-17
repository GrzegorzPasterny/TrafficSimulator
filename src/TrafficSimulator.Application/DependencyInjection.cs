using Microsoft.Extensions.DependencyInjection;

namespace TrafficSimulator.Application
{
	public static class DependencyInjection
	{
		public static ServiceCollection AddApplication(this ServiceCollection services)
		{
			services.AddMediatR(options =>
			{
				options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
			});

			return services;
		}
	}
}
