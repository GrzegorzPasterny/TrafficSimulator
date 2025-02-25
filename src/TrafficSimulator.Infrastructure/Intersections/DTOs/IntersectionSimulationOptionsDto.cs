namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionSimulationOptionsDto
	{
		public int MinimalDistanceBetweenTheCars { get; set; }
		public double StepTimespanMs { get; set; }
		public double TimeoutMs { get; set; }
		public int StepLimit { get; set; }
	}
}
