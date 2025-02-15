using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Infrastructure.Errors;

namespace TrafficSimulator.Infrastructure.CarGenerators.Generators
{
	public class SingleCarGenerator : CarGenerator
	{
		public SingleCarGeneratorOptions Options { get; } = new();
		private bool _hasFinished = false;
		private bool _wasStarted = false;
		private Task? _carGenerationTask;
		private readonly ISender _mediator;

		public SingleCarGenerator(Intersection root, IntersectionObject? parent, ISender mediator)
			: base(root, parent)
		{
			_mediator = mediator;
		}

		public override ErrorOr<bool> IsGenerationFinished()
		{
			return _hasFinished;
		}

		public override UnitResult<Error> StartGenerating()
		{
			if (_wasStarted)
			{
				return InfrastructureErrors.CarGeneratorAlreadyStarted();
			}

			_wasStarted = true;
			_carGenerationTask = Task.Run(GenerateCars);

			return UnitResult.Success<Error>();
		}

		public override UnitResult<Error> StopGenerating()
		{
			throw new NotImplementedException();
		}

		private async Task GenerateCars()
		{
			await Task.Delay(Options.DelayForGeneratingTheCar);

			Car car = new Car((InboundLane)Parent!);

			var command = new AddCarCommand(car);

			await _mediator.Send(command);

			_hasFinished = true;
		}
	}
}
