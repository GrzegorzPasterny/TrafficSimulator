using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class InboundLaneDto : OutboundLaneDto
	{
		public List<TurnPossibilityDto> TurnPossibilities { get; set; }
		public string CarGeneratorTypeName { get; set; }
		public object[] CarGeneratorOptions { get; set; }
		public LaneType[] LaneTypes { get; set; }
		public bool ContainsTrafficLights { get; set; }
	}
}
