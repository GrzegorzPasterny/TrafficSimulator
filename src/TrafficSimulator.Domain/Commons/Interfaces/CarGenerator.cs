using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public abstract class CarGenerator : IntersectionObject, ICarGenerator
	{
		public CarGenerator(Intersection root) : base(root)
		{
		}

		public abstract ErrorOr<bool> IsGenerationFinished();
		public abstract UnitResult<Error> StartGenerating();
		public abstract UnitResult<Error> StopGenerating();

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{typeof(CarGenerator).Name}";
		}
	}
}
