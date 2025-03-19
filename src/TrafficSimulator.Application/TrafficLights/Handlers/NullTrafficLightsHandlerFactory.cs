using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.Lights;
using TrafficSimulator.Application.Lights.HandlerTypes;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers
{
	public class NullTrafficLightsHandlerFactory : ITrafficLightsHandlerFactory
	{
		private TrafficPhase? _currentPhase;

		public NullTrafficLightsHandlerFactory()
		{
		}
		public NullTrafficLightsHandlerFactory(TrafficPhase? currentPhase)
		{
			_currentPhase = currentPhase;
		}

		public ITrafficLightsHandler CreateHandler(string mode = "Sequential")
		{
			return new NullTrafficLightsHandler(_currentPhase);
		}
	}
}
