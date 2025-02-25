using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class TurnPossibilityDto
	{
		public LaneType LaneType { get; set; }
		public bool ContainsTrafficLights { get; set; }
	}
}
