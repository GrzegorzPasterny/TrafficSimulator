﻿using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public class MultipleCarsGenerator : CarGenerator<MultipleCarsGeneratorOptions>
	{
		private bool _isGenerationCompleted = false;
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

		public override bool IsGenerationCompleted => _isGenerationCompleted;

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			if (_isGenerationCompleted)
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
			_isGenerationCompleted = false;
			_timeFromLastGeneration = TimeSpan.Zero;
			_carsGeneratedSoFar = 0;

		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationCompleted}]";
		}
	}
}
