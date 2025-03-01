using MediatR;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Cars.GetCars
{
	public record GetCarsCommand() : IRequest<IEnumerable<Car>>
	{
	}
}
