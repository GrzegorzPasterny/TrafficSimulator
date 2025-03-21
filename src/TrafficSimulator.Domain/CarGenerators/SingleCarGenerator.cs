using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public class SingleCarGenerator : CarGenerator<SingleCarGeneratorOptions>
	{
		private TimeSpan _simulationTime = TimeSpan.Zero;
		private bool _isGenerationFinished = false;

		public SingleCarGenerator(Intersection root, IntersectionObject? parent, ISender mediator, SingleCarGeneratorOptions? singleCarGeneratorOptions = null)
			: base(root, parent, mediator)
		{
			if (singleCarGeneratorOptions is not null)
			{
				Options = singleCarGeneratorOptions;
			}
			else
			{
				Options = new SingleCarGeneratorOptions();
			}
		}

		public override bool IsGenerationCompleted => _isGenerationFinished;

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

		public override void Reset()
		{
			_isGenerationFinished = false;
			_simulationTime = TimeSpan.Zero;
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationFinished}]";
		}
	}
}
