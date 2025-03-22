using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.CarGenerators
{
	public class WaveCarsGenerator : CarGenerator<WaveCarsGeneratorOptions>
	{
		private int _carsGeneratedSoFar = 0;
		private bool _isGenerationCompleted = false;
		private DateTime _startTime = DateTime.UtcNow;

		public WaveCarsGenerator(Intersection root, IntersectionObject? parent, ISender mediator, WaveCarsGeneratorOptions options)
			: base(root, parent, mediator)
		{
			Options = options;
		}

		public override bool IsGenerationCompleted => _isGenerationCompleted;

		public override async Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			if (_isGenerationCompleted)
			{
				return UnitResult.Success<Error>();
			}

			// Compute the final adjusted probability
			double carGenerationProbability = GetAdjustedCarGenerationProbability(timeSpan);

			// Determine whether to generate a car
			if (ShouldGenerateCar(carGenerationProbability))
			{
				await GenerateCar();
				_carsGeneratedSoFar++;
			}

			// Check if generation should be marked as completed
			DetermineGenerationCompletion();

			return UnitResult.Success<Error>();
		}

		/// <summary>
		/// Computes the probability of generating a car in this step, considering base rate and wave fluctuation.
		/// </summary>
		private double GetAdjustedCarGenerationProbability(TimeSpan timeSpan)
		{
			double baseProbability = Options.BaseRate * timeSpan.TotalSeconds * 100;

			double waveEffect = CalculateWaveEffect();

			double adjustedProbability = baseProbability + (waveEffect * timeSpan.TotalSeconds);

			return Math.Clamp(adjustedProbability, 0, 100);
		}

		/// <summary>
		/// Simulates a sinusoidal fluctuation in car generation probability.
		/// </summary>
		private double CalculateWaveEffect()
		{
			double elapsedSeconds = (DateTime.UtcNow - _startTime).TotalSeconds;
			return Math.Sin(elapsedSeconds * Options.WavePeriodHz * 2 * Math.PI) * Options.WaveAmplitude;
		}

		/// <summary>
		/// Determines whether a car should be generated based on the computed probability.
		/// </summary>
		private bool ShouldGenerateCar(double probability)
		{
			return Random.Shared.NextDouble() * 100 <= probability;
		}

		public override void Reset()
		{
			_carsGeneratedSoFar = 0;
			_isGenerationCompleted = false;
			_startTime = DateTime.UtcNow;
		}

		private void DetermineGenerationCompletion()
		{
			if (_carsGeneratedSoFar >= Options.AmountOfCarsToGenerate)
			{
				_isGenerationCompleted = true;
			}
		}
	}
}
