using MediatR;

namespace TrafficSimulator.Application.Cars.StopCar
{
	public record StopCarCommand(long carId, object lane) : INotification;
}
