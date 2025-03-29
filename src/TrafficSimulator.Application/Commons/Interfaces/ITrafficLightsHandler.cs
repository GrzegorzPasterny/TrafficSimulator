using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ITrafficLightsHandler
	{
		void LoadIntersection(Intersection intersection);
		Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed);
		TrafficPhase? GetCurrentTrafficPhase();
		UnitResult<Error> SetLightsManually(string trafficPhaseName);
	}
}
