using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.Intersection;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public abstract class CarGenerator : IntersectionObject, ICarGenerator
	{
		public CarGenerator(Intersection root, string name) : base(root, name)
		{
		}

		public abstract ErrorOr<bool> IsGenerationFinished();
		public abstract UnitResult<Error> StartGenerating();
		public abstract UnitResult<Error> StopGenerating();
	}
}
