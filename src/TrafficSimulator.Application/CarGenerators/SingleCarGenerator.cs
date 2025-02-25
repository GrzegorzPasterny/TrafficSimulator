using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.Handlers.CarGenerators
{
	public class SingleCarGenerator : CarGenerator
	{
		public SingleCarGeneratorOptions Options { get; } = new();
		private TimeSpan _simulationTime = TimeSpan.Zero;
		private bool _isGenerationFinished = false;

		public SingleCarGenerator(Intersection root, IntersectionObject? parent, ISender mediator, SingleCarGeneratorOptions? singleCarGeneratorOptions = null)
			: base(root, parent, mediator)
		{
			if (singleCarGeneratorOptions is not null)
			{
				Options = singleCarGeneratorOptions;
			}
		}

		public override bool IsGenerationFinished => _isGenerationFinished;

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			_simulationTime = _simulationTime.Add(timeSpan);

			if (_simulationTime >= Options.DelayForGeneratingTheCar)
			{
				await GenerateCar();

				_isGenerationFinished = true;
			}

			return UnitResult.Success<Error>();
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationFinished}]";
		}
	}
}
