namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionSimulationDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IntersectionDto Intersection { get; set; }
		public IntersectionSimulationOptionsDto Options { get; set; }
	}
}
