using MediatR;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Cars.AddCar
{
	public record AddCarCommand(Car Car) : IRequest;
}
