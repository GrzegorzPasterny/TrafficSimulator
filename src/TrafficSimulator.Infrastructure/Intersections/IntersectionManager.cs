using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Intersection;
using TrafficSimulator.Infrastructure.Errors;

namespace TrafficSimulator.Infrastructure.Intersections
{
    public class IntersectionManager : IIntersectionProvider, IIntersectionRepository
	{
		private Intersection? _currentIntersection;

		public ErrorOr<Intersection> GetCurrentIntersection()
		{
			if (_currentIntersection == null)
			{
				return InfrastructureErrors.IntersectionNotSet();
			}

			return _currentIntersection;
		}

		public UnitResult<Error> SetCurrentIntersection(Intersection intersection)
		{
			_currentIntersection = intersection;

			return UnitResult.Success<Error>();
		}
	}
}
