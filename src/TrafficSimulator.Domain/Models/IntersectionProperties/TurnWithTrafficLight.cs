using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Models.IntersectionProperties
{
	public class TurnWithTrafficLight
	{
		public InboundLane? InboundLane { get; set; }
		public TurnPossibility? TurnPossibility { get; set; }
		public TrafficLightState TrafficLightState { get; set; }

		public override string ToString()
		{
			return $"[InboundLaneName = {InboundLane?.Name}, " +
				$"TurnType = {TurnPossibility?.LaneType}, " +
				$"TrafficLightsState = {TrafficLightState}]";
		}
	}
}
