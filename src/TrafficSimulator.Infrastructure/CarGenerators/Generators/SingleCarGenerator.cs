using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.Intersection;
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
		private readonly Lane _carStartLocation;
		private readonly IIntersectionProvider _intersectionProvider;

		public SingleCarGenerator(Intersection root, string name,
			ISender mediator, Lane carStartLocation, IIntersectionProvider intersectionProvider)
			: base(root, name)
		{
			_mediator = mediator;
			_carStartLocation = carStartLocation;
			_intersectionProvider = intersectionProvider;
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

		private async Task GenerateCars()
		{
			await Task.Delay(Options.DelayForGeneratingTheCar);

			ErrorOr<Intersection> intersectionResult = _intersectionProvider.GetCurrentIntersection();

			if (intersectionResult.IsError)
			{
				// TODO: Handle
			}

			Intersection intersection = intersectionResult.Value;

			Car car = new Car(_carStartLocation);

			var command = new AddCarCommand(car);

			await _mediator.Send(command);

			_hasFinished = true;
		}

		public override UnitResult<Error> StopGenerating()
		{
			throw new NotImplementedException();
		}
	}
}
