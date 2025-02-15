using ErrorOr;
using TrafficSimulator.Domain.Models.Intersection;

namespace TrafficSimulator.Application.Commons.Interfaces
{
    public interface IIntersectionProvider
	{
		ErrorOr<Intersection> GetCurrentIntersection();
	}
}
