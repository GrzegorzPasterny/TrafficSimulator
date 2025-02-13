using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class Lane : LocationEntity
	{
		public Lane(int distance = 10) : base(distance)
		{
		}

		//public ICarGenerator CarGenerator { get; private set; }

		//public UnitResult<Error> AddCarGenerator(ICarGenerator carGenerator)
		//{
		//	// TODO: Handle error paths
		//	CarGenerator = carGenerator;
		//	return UnitResult.Success<Error>();
		//}
	}
}
