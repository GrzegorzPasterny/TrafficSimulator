using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers.Lights
{
	public class NullTrafficLightsHandler : ITrafficLightsHandler
	{
		public void LoadIntersection(Intersection intersection)
		{
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			return Task.FromResult(UnitResult.Success<Error>());
		}
	}
}
