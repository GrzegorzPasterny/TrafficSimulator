using ErrorOr;
using MediatR;
using System.Text.Json;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.CarGenerators
{
	public class CarGeneratorFactory
	{
		private readonly Dictionary<string, Type> _generatorTypes = new()
		{
			{ nameof(SingleCarGenerator), typeof(SingleCarGenerator) },
			{ nameof(MultipleCarsGenerator), typeof(MultipleCarsGenerator) },
			{ nameof(RandomCarsGenerator), typeof(RandomCarsGenerator) },
			{ nameof(WaveCarsGenerator), typeof(WaveCarsGenerator) },
		};

		private readonly Dictionary<string, Type> _optionsTypes = new()
		{
			{ nameof(SingleCarGenerator), typeof(SingleCarGeneratorOptions) },
			{ nameof(MultipleCarsGenerator), typeof(MultipleCarsGeneratorOptions) },
			{ nameof(RandomCarsGenerator), typeof(RandomCarsGeneratorOptions) },
			{ nameof(WaveCarsGenerator), typeof(WaveCarsGeneratorOptions) },
		};
		private readonly ISender _mediator;

		public CarGeneratorFactory(ISender mediator)
		{
			_mediator = mediator;
		}

		public ErrorOr<ICarGenerator?> Create(string carGeneratorType, JsonElement carGeneratorOptions, Intersection root, IntersectionObject parent)
		{
			ICarGenerator? carGenerator = null;

			if (string.IsNullOrWhiteSpace(carGeneratorType))
			{
				return carGenerator.ToErrorOr();
			}

			if (!_generatorTypes.TryGetValue(carGeneratorType, out var generatorType)
				|| !_optionsTypes.TryGetValue(carGeneratorType, out var optionsType)
				)
			{
				// TODO: Return ApplicationError here
				throw new Exception($"Unknown CarGenerator type: {carGeneratorType}");
			}

			// Deserialize options using the correct type
			var options = (CarGeneratorOptions?)JsonSerializer.Deserialize(carGeneratorOptions, optionsType);

			if (options is null)
			{
				// TODO: Return ApplicationError here
				throw new Exception($"Failed to deserialize options for {carGeneratorType}");
			}

			carGenerator = (ICarGenerator?)Activator.CreateInstance(generatorType, root, parent, _mediator, options);

			return carGenerator.ToErrorOr();
		}
	}
}
