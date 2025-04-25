using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.CarGenerators.DomainEvents;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public abstract class CarGenerator<TOptions> : IntersectionObject, ICarGenerator
		where TOptions : CarGeneratorOptions, new()
	{
		private readonly ISender _mediator;

		public CarGenerator(Intersection root, IntersectionObject? parent, ISender mediator) : base(root, parent)
		{
			_mediator = mediator;
		}

		public abstract bool IsGenerationCompleted { get; }

		public TOptions Options { get; init; }

		CarGeneratorOptions ICarGenerator.Options => Options;

		public abstract Task<UnitResult<Error>> Generate(TimeSpan timeSpan);

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {IsGenerationCompleted}]";
		}

		public abstract void Reset();

		protected virtual async Task GenerateCar()
		{
			Car car = new((InboundLane)Parent!, Options.CarOptions);

			var command = new AddCarCommandDomainEvent(car);

			await _mediator.Send(command);
		}
	}
}
