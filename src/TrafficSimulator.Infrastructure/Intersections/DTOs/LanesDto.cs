using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class LanesDto : IntersectionObjectDto
	{
		public List<InboundLaneDto> InboundLanes { get; set; }
		public List<OutboundLaneDto> OutboundLanes { get; set; }
		public WorldDirection WorldDirection { get; set; }

	}
}
