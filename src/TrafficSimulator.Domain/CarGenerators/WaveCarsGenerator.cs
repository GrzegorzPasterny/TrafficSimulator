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
		private double _currentProbability = 0; // Probability changes over time
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

			double frequency = Options.WavePeriodHz;
			double intensity = Options.WaveAmplitude;
			double baseProbability = Options.BaseProbability;

			_currentProbability = ComputeProbability(DateTime.UtcNow - _startTime, frequency, intensity, baseProbability);

			if (Random.Shared.NextDouble() * 100 <= _currentProbability)
			{
				await GenerateCar();
				_carsGeneratedSoFar++;
			}

			if (_carsGeneratedSoFar >= Options.TotalCarsToGenerate)
			{
				_isGenerationCompleted = true;
			}

			return UnitResult.Success<Error>();
		}

		public override void Reset()
		{
			_carsGeneratedSoFar = 0;
			_isGenerationCompleted = false;
			_startTime = DateTime.UtcNow;
		}

		private double ComputeProbability(TimeSpan elapsedTime, double frequency, double intensity, double baseProbability)
		{
			double timeInSeconds = elapsedTime.TotalSeconds;

			// Probability oscillates in waves (sinusoidal function)
			double waveEffect = Math.Sin(timeInSeconds * frequency) * intensity;

			return Math.Clamp(baseProbability + waveEffect, 0, 100);
		}
	}

}
