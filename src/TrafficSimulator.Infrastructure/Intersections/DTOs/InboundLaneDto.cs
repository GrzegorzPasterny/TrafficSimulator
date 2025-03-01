using System.Text.Json;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class InboundLaneDto : OutboundLaneDto
	{
		public string CarGeneratorTypeName { get; set; }
		public LaneType[] LaneTypes { get; set; }
		public bool ContainsTrafficLights { get; set; }

		/// <summary>
		/// Options for <see cref="CarGeneratorTypeName"/> saved in json format
		/// </summary>
		public JsonElement CarGeneratorOptions { get; set; }
	}
}
