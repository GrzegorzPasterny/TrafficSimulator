using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ITrafficLightsHandler
	{
		void LoadIntersection(Intersection intersection);
		Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed);
	}
}
