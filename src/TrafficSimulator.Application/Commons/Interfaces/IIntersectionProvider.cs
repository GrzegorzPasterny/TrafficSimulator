using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface IIntersectionProvider
	{
		ErrorOr<Intersection> GetCurrentIntersection();
	}
}
