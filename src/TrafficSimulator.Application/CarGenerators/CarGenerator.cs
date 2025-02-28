using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.AddCar;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers.CarGenerators
{
	public abstract class CarGenerator<TOptions> : IntersectionObject, ICarGenerator
		where TOptions : CarGeneratorOptions, new()
	{
		private readonly ISender _mediator;

		public CarGenerator(Intersection root, IntersectionObject? parent, ISender mediator) : base(root, parent)
		{
			_mediator = mediator;
		}

		public abstract bool IsGenerationFinished { get; }

		public TOptions Options { get; init; }

		CarGeneratorOptions ICarGenerator.Options => Options;

		public abstract Task<UnitResult<Error>> Generate(TimeSpan timeSpan);

		internal async Task GenerateCar()
		{
			Car car = new Car((InboundLane)Parent!);

			var command = new AddCarCommand(car);

			await _mediator.Send(command);
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {IsGenerationFinished}]";
		}
	}
}
