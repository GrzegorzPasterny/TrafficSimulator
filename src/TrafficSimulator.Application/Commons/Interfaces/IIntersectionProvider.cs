using ErrorOr;
using TrafficSimulator.Domain.IntersectionObjects;

namespace TrafficSimulator.Application.Commons.Interfaces
{
    public interface IIntersectionProvider
	{
		ErrorOr<Intersection> GetCurrentIntersection();
	}
}
