using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Handlers.CarGenerators;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Application.CarGenerators
{
	public class CarGeneratorFactory
	{
		private readonly Dictionary<string, Type> _generatorTypes = new()
		{
			{ nameof(SingleCarGenerator), typeof(SingleCarGenerator) },
			{ nameof(MultipleCarsGenerator), typeof(MultipleCarsGenerator) }
		};

		private readonly Dictionary<string, Type> _optionsTypes = new()
		{
			{ nameof(SingleCarGenerator), typeof(SingleCarGeneratorOptions) },
			{ nameof(MultipleCarsGenerator), typeof(MultipleCarsGeneratorOptions) },
		};
		private readonly ISender _mediator;

		public CarGeneratorFactory(ISender mediator)
		{
			_mediator = mediator;
		}

		public ErrorOr<ICarGenerator?> Create(string carGeneratorType, Intersection root, IntersectionObject parent)
		{
			ICarGenerator? carGenerator = null;

			if (string.IsNullOrWhiteSpace(carGeneratorType))
			{
				return carGenerator.ToErrorOr();
			}

			if (!_generatorTypes.TryGetValue(carGeneratorType, out var generatorType)
				//|| !_optionsTypes.TryGetValue(typeName, out var optionsType)
				)
			{
				// TODO: Return ApplicationError here
				throw new Exception($"Unknown CarGenerator type: {carGeneratorType}");
			}

			carGenerator = (ICarGenerator?)Activator.CreateInstance(generatorType, root, parent, _mediator, null);

			return carGenerator.ToErrorOr();
		}
	}
}
