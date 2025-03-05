using Microsoft.Extensions.DependencyInjection;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Simulation
{
	public class IntersectionSimulationHandlerFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public IntersectionSimulationHandlerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ISimulationHandler CreateHandler(string mode)
		{
			return mode switch
			{
				SimulationMode.InMemory => _serviceProvider.GetRequiredService<InMemoryIntersectionSimulationHandler>(),
				SimulationMode.RealTime => _serviceProvider.GetRequiredService<RealTimeIntersectionSimulationHandler>(),
				_ => throw new ArgumentException($"Invalid simulation mode: {mode}", nameof(mode))
			};
		}
	}
}
