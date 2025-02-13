using MediatR;

namespace TrafficSimulator.Application.Cars.StartCar
{
	public record StartCarCommand(long carId, object lane) : INotification;
}
