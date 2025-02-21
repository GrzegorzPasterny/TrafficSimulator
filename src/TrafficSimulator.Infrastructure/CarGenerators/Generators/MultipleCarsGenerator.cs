using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Infrastructure.CarGenerators.Generators
{
	public class MultipleCarsGenerator : CarGenerator
	{
		public MultipleCarsGeneratorOptions Options { get; } = new();
		private bool _isGenerationFinished = false;
		private readonly ISender _mediator;
		private TimeSpan _simulationTime = TimeSpan.Zero;

		public MultipleCarsGenerator(
			Intersection root, IntersectionObject? parent,
			ISender mediator, MultipleCarsGeneratorOptions? multipleCarsGeneratorOptions = null)
			: base(root, parent)
		{
			_mediator = mediator;

			if (multipleCarsGeneratorOptions is not null)
			{
				Options = multipleCarsGeneratorOptions;
			}
		}

		public override bool IsGenerationFinished => _isGenerationFinished;

		public override Task<UnitResult<Error>> Generate(TimeSpan timeSpan)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {_isGenerationFinished}]";
		}
	}
}
