namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionDto : IntersectionObjectDto
	{
		public List<LanesDto> LanesCollection { get; set; }
		public IntersectionCoreDto? IntersectionCore { get; set; }
		public List<TrafficPhaseDto> TrafficPhases { get; set; }

	}
}
