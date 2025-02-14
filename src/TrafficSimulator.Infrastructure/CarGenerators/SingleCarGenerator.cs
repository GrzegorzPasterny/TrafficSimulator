using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Infrastructure.Errors;

namespace TrafficSimulator.Infrastructure.CarGenerators
{
	public class SingleCarGenerator : ICarGenerator
	{
		public SingleCarGeneratorOptions Options { get; } = new();
		private bool _hasFinished = false;
		private bool _wasStarted = false;
		private Task? _carGenerationTask;
		private readonly ISender _mediator;
		private readonly Lane _carStartLocation;
		private readonly IIntersectionProvider _intersectionProvider;

		public SingleCarGenerator(ISender mediator, Lane carStartLocation, IIntersectionProvider intersectionProvider)
		{
			_mediator = mediator;
			_carStartLocation = carStartLocation;
			_intersectionProvider = intersectionProvider;
		}

		public ErrorOr<bool> IsGenerationFinished()
		{
			return _hasFinished;
		}

		public UnitResult<Error> StartGenerating()
		{
			if (_wasStarted)
			{
				return InfrastructureErrors.CarGeneratorAlreadyStarted();
			}

			_wasStarted = true;
			_carGenerationTask = Task.Run(GenerateCars);

			return UnitResult.Success<Error>();
		}

		private async Task GenerateCars()
		{
			await Task.Delay(Options.DelayForGeneratingTheCar);

			ErrorOr<Intersection> intersectionResult = _intersectionProvider.GetCurrentIntersection();

			if (intersectionResult.IsError)
			{
				// TODO: Handle
			}

			Intersection intersection = intersectionResult.Value;

			LaneType carTurnType = _carStartLocation.LaneType.First();

			WorldDirection outboundLaneWorldDirection = _carStartLocation.WorldDirection.Rotate(carTurnType);

			Lanes? outboundLanes = intersection.Lanes.Find(lanes => lanes.WorldDirection == outboundLaneWorldDirection);

			if (outboundLanes is null || outboundLanes.OutboundLanes?.Count == 0)
			{
				// TODO: Handle
			}

			Lane? carEndLocation = outboundLanes?.OutboundLanes?.First();

			if (carEndLocation is null)
			{
				// TODO: Handle
			}

			Car car = new Car(_carStartLocation, carEndLocation);

			var command = new AddCarCommand(car);

			await _mediator.Send(command);

			_hasFinished = true;
		}

		public UnitResult<Error> StopGenerating()
		{
			throw new NotImplementedException();
		}
	}
}
