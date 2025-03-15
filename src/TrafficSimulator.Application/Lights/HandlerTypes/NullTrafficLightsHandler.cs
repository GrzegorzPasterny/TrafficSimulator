using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	public class NullTrafficLightsHandler : ITrafficLightsHandler
	{
		public TrafficPhase GetCurrentTrafficPhase()
		{
			throw new NotImplementedException();
		}

		public void LoadIntersection(Intersection intersection)
		{
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			return Task.FromResult(UnitResult.Success<Error>());
		}
	}
}
