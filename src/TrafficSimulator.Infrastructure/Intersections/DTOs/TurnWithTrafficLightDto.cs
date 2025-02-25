using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class TurnWithTrafficLightDto
	{
		public string InboundLaneName { get; set; }
		public TurnPossibilityDto TurnPossibility { get; set; }
		public TrafficLightState TrafficLightState { get; set; }
	}
}
