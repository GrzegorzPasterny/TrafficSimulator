using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Infrastructure.Errors;

namespace TrafficSimulator.Infrastructure.CarGenerators
{
	public class SingleCarGenerator : ICarGenerator
	{
		public SingleCarGeneratorOptions Options { get; } = new();
		private bool _hasFinished = false;
		private bool _wasStarted = false;
		private readonly ISender _mediator;
		private Task? _carGenerationTask;

		public SingleCarGenerator(ISender mediator)
		{
			_mediator = mediator;
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

			Car car = new Car();

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
