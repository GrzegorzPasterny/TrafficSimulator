using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	public class NullTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly TrafficPhase? _trafficPhase;

		public NullTrafficLightsHandler()
		{
		}

		public NullTrafficLightsHandler(TrafficPhase? trafficPhase)
		{
			_trafficPhase = trafficPhase;
		}

		public TrafficPhase GetCurrentTrafficPhase()
		{
			return _trafficPhase;
		}

		public void LoadIntersection(Intersection intersection)
		{
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			return Task.FromResult(UnitResult.Success<Error>());
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			return UnitResult.Success<Error>();
		}
	}
}
