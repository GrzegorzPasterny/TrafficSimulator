using MediatR;

namespace TrafficSimulator.Application.Cars.MoveCar
{
	public record MoveAllCarsCommand(TimeSpan StepTimespan) : IRequest;
}
