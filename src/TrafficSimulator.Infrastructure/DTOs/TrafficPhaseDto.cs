using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class TrafficPhaseDto
	{
		public string Name { get; set; }
		public List<TurnWithTrafficLight> TrafficLightsAssignments { get; set; }

	}
}
