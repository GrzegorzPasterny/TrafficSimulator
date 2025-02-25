using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Application.Handlers.Lights
{
	public class NullTrafficLightsHandler : ITrafficLightsHandler
	{
		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			return Task.FromResult(UnitResult.Success<Error>());
		}
	}
}
