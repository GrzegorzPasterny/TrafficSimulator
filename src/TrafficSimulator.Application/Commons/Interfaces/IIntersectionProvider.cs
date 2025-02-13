using ErrorOr;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface IIntersectionProvider
	{
		ErrorOr<Intersection> GetCurrentIntersection();
	}
}
