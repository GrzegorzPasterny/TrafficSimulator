using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.Nest
{
	public class NestTrafficLightsHandler : ITrafficLightsHandler
	{
		public TrafficPhase? GetCurrentTrafficPhase()
		{
			throw new NotImplementedException();
		}

		public void LoadIntersection(Intersection intersection)
		{
			throw new NotImplementedException();
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			throw new NotImplementedException();
		}
	}
}
