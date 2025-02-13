using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models
{
	public class Lane : Entity
	{
		public ICarGenerator CarGenerator { get; private set; }

		public UnitResult<Error> AddCarGenerator(ICarGenerator carGenerator)
		{
			// TODO: Handle error paths
			CarGenerator = carGenerator;
			return UnitResult.Success<Error>();
		}
	}
}
