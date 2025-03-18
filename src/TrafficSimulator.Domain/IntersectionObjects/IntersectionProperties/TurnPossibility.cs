using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Models.IntersectionProperties
{
	public class TurnPossibility
	{
		public LaneType LaneType { get; set; }

		// TODO: Intersection Builder should assign here Traffic Lights.
		// TODO: Should be Get only
		public TrafficLight? TrafficLights { get; set; }

		// TODO: Should be Get only
		public bool ContainsTrafficLights { get; set; }

		public override string ToString()
		{
			return $"[LaneType = {LaneType}, " +
				$"TrafficLights = {TrafficLights}]";
		}
	}
}
