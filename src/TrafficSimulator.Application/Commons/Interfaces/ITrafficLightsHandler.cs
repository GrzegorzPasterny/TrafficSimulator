using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ITrafficLightsHandler
	{
		Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed);
	}
}
