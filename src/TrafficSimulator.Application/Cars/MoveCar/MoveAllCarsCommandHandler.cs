using MediatR;
using TrafficSimulator.Application.Cars.CommitCarsMovement;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Cars.MoveCar
{
	public class MoveAllCarsCommandHandler : IRequestHandler<MoveAllCarsCommand>
	{
		private readonly ISender _sender;

		public MoveAllCarsCommandHandler(ISender sender)
		{
			_sender = sender;
		}

		public async Task Handle(MoveAllCarsCommand request, CancellationToken cancellationToken)
		{
			IEnumerable<Car> cars = await _sender.Send(new GetCarsCommand());

			// TODO: Can be parallelized?
			foreach (var car in cars)
			{
				car.Move(request.StepTimespan, cars);
			}

			await _sender.Send(new CommitCarsMovementCommand());
		}
	}
}
