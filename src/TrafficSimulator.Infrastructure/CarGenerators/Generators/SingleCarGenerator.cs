using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Infrastructure.CarGenerators.Generators
{
	public class SingleCarGenerator : CarGenerator
	{
		public SingleCarGeneratorOptions Options { get; } = new();
		private bool _isGenerationFinished = false;
		private readonly ISender _mediator;
		private TimeSpan _simulationTime = TimeSpan.Zero;

		public SingleCarGenerator(Intersection root, IntersectionObject? parent, ISender mediator, SingleCarGeneratorOptions? singleCarGeneratorOptions = null)
			: base(root, parent)
		{
			_mediator = mediator;

			if (singleCarGeneratorOptions is not null)
			{
				Options = singleCarGeneratorOptions;
			}
		}

		public override bool IsGenerationFinished => _isGenerationFinished;

		private async Task GenerateCar()
		{
			Car car = new Car((InboundLane)Parent!);

			var command = new AddCarCommand(car);

			await _mediator.Send(command);

			_isGenerationFinished = true;
		}

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			_simulationTime = _simulationTime.Add(timeSpan);

			if (_simulationTime >= Options.DelayForGeneratingTheCar)
			{
				await GenerateCar();
			}

			return UnitResult.Success<Error>();
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationFinished}]";
		}
	}
}
