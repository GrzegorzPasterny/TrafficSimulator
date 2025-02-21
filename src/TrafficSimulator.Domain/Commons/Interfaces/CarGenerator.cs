using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public abstract class CarGenerator : IntersectionObject, ICarGenerator
	{
		public CarGenerator(Intersection root, IntersectionObject? parent) : base(root, parent)
		{
		}

		public abstract bool IsGenerationFinished { get; }

		public abstract Task<UnitResult<Error>> Generate(TimeSpan timeSpan);

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{typeof(CarGenerator).Name}";
		}

		public override string ToString()
		{
			return $"[CarsGeneratorName = {Name}, HasFinished = {IsGenerationFinished}]";
		}
	}
}
