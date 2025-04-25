using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.CarGenerators
{
	public class RandomCarsGenerator : CarGenerator<RandomCarsGeneratorOptions>
	{
		private bool _isGenerationCompleted = false;
		private int _carsGeneratedSoFar = 0;
		private readonly Random _randomNumberGenerator = new();

		public RandomCarsGenerator(
			Intersection root, IntersectionObject? parent,
			ISender mediator, RandomCarsGeneratorOptions? randomCarsGeneratorOptions = null)
			: base(root, parent, mediator)
		{
			if (randomCarsGeneratorOptions is not null)
			{
				Options = randomCarsGeneratorOptions;
			}
			else
			{
				Options = new RandomCarsGeneratorOptions();
			}
		}

		public override bool IsGenerationCompleted => _isGenerationCompleted;

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			if (_isGenerationCompleted)
			{
				// TODO: It is possible to return Error here maybe..
				return UnitResult.Success<Error>();
			}

			double baseProbability = Options.BaseRate * timeSpan.TotalSeconds * 100;

			int randomValue = _randomNumberGenerator.Next(0, 100);

			if (randomValue <= baseProbability)
			{
				await GenerateCar();

				_carsGeneratedSoFar++;
			}

			DetermineGenerationCompletion();

			return UnitResult.Success<Error>();
		}

		private void DetermineGenerationCompletion()
		{
			if (_carsGeneratedSoFar >= Options.AmountOfCarsToGenerate)
			{
				_isGenerationCompleted = true;
			}
		}

		public override void Reset()
		{
			_carsGeneratedSoFar = 0;
			_isGenerationCompleted = false;
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationCompleted}]";
		}
	}
}
