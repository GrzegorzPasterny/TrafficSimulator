using ErrorOr;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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

		private readonly IServiceProvider? _serviceProvider;
		private readonly ISender? _sender;

		public CarGeneratorFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public CarGeneratorFactory(ISender sender)
		{
			_sender = sender;
		}

		public CarGeneratorFactory()
		{
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

			if (_serviceProvider is not null)
			{
				carGenerator = (ICarGenerator?)ActivatorUtilities.CreateInstance(_serviceProvider, generatorType, root, parent, options);
			}
			else if (_sender is not null)
			{
				carGenerator = (ICarGenerator?)Activator.CreateInstance(generatorType, root, parent, _sender, options);
			}
			else
			{
				throw new Exception("Failed to craete Car Generator object");
			}

			return carGenerator.ToErrorOr();
		}
	}
}
