using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface IIntersectionRepository
	{
		UnitResult<Error> SetCurrentIntersection(Intersection intersection);
	}
}
