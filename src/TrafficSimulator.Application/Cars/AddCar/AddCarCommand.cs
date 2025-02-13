using MediatR;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Cars.AddCar
{
	public record AddCarCommand(Car Car) : INotification;
}
