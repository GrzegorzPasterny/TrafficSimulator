using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Application.Cars.GetCars
{
	public class GetCarsResponse
	{
		public IEnumerable<Car> Cars { get; set; }
	}
}
