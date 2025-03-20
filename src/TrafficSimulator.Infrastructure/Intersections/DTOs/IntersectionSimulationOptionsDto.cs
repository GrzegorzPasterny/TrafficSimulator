using TrafficSimulator.Domain.Cars;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionSimulationOptionsDto
	{
		public CarOptions CarOptions { get; set; }
		public double StepTimespanMs { get; set; }
		public double TimeoutMs { get; set; }
		public int StepLimit { get; set; }
		public string? TrafficLightHandlerType { get; set; }
	}
}
