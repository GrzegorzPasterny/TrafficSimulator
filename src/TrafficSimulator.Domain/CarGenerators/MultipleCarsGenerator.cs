using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public class MultipleCarsGenerator : CarGenerator<MultipleCarsGeneratorOptions>
	{
		public MultipleCarsGeneratorOptions Options { get; } = new();
		private bool _isGenerationFinished = false;
		private TimeSpan _simulationTime = TimeSpan.Zero;
		private TimeSpan _timeFromLastGeneration = TimeSpan.Zero;
		private int _carsGeneratedSoFar = 0;

		public MultipleCarsGenerator(
			Intersection root, IntersectionObject? parent,
			ISender mediator, MultipleCarsGeneratorOptions? multipleCarsGeneratorOptions = null)
			: base(root, parent, mediator)
		{
			if (multipleCarsGeneratorOptions is not null)
			{
				Options = multipleCarsGeneratorOptions;
			}
			else
			{
				Options = new MultipleCarsGeneratorOptions();
			}
		}

		public override bool IsGenerationFinished => _isGenerationFinished;

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			if (_isGenerationFinished)
			{
				// TODO: It is possible to return Error here maybe..
				return UnitResult.Success<Error>();
			}

			_timeFromLastGeneration += timeSpan;

			if (_carsGeneratedSoFar == 0 || _timeFromLastGeneration > Options.DelayBetweenCarGeneration)
			{
				await GenerateCar();

				_carsGeneratedSoFar++;
				_timeFromLastGeneration = TimeSpan.Zero;
			}

			if (_carsGeneratedSoFar >= Options.AmountOfCarsToGenerate)
			{
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
