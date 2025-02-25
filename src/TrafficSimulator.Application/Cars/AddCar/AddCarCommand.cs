using MediatR;
using TrafficSimulator.Domain.Cars;

namespace TrafficSimulator.Application.Cars.AddCar
{
    public record AddCarCommand(Car Car) : IRequest;
}
