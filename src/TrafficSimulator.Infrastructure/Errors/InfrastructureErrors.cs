using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Infrastructure.Errors
{
	public static class InfrastructureErrors
	{
		public static Error IntersectionNotSet()
			=> Error.NotFound("TrafficSimulator.Intersection.NotSet", "Intersection has not been set");

		internal static UnitResult<Error> CarGeneratorAlreadyStarted()
		{
			throw new NotImplementedException();
		}
	}
}
