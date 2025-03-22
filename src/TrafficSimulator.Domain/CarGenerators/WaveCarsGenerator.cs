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

			double currentProbability = ComputeProbability(DateTime.UtcNow - _startTime, Options);

			double adjustedProbability = AdjustProbabilityForSimulationStepTimeSpan(currentProbability, timeSpan);

			if (Random.Shared.NextDouble() * 100 <= adjustedProbability)
			{
				await GenerateCar();
				_carsGeneratedSoFar++;
			}

			DetermineGenerationCompletion();

			return UnitResult.Success<Error>();
		}

		private double AdjustProbabilityForSimulationStepTimeSpan(double currentProbability, TimeSpan timeSpan)
		{
			// Adjust probability based on how often Generate() is called
			double scalingFactor = Math.Min(1, timeSpan.TotalSeconds * Options.WavePeriodHz);
			double adjustedProbability = currentProbability * scalingFactor;
			return adjustedProbability;
		}

		public override void Reset()
		{
			_carsGeneratedSoFar = 0;
			_isGenerationCompleted = false;
			_startTime = DateTime.UtcNow;
		}

		private double ComputeProbability(TimeSpan elapsedTime, WaveCarsGeneratorOptions options)
		{
			double timeInSeconds = elapsedTime.TotalSeconds;

			// Compute wave effect
			double waveEffect = Math.Sin(timeInSeconds * options.WavePeriodHz * 2 * Math.PI) * options.WaveAmplitude;

			// Ensure that probability remains valid
			return Math.Max(0, options.BaseRate + waveEffect);
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
