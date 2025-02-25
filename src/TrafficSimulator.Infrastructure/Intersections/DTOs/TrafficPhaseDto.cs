namespace TrafficSimulator.Infrastructure.DTOs
{
	public class TrafficPhaseDto
	{
		public string Name { get; set; }
		public List<TurnWithTrafficLightDto> TrafficLightsAssignments { get; set; }

	}
}
