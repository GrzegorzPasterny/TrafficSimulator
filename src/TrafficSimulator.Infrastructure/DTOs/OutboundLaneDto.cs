using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class OutboundLaneDto : LocationEntityDto
	{
		public WorldDirection WorldDirection { get; }

	}
}
