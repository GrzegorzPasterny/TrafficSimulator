using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane
	{
		public InboundLane(
			Intersection root, IntersectionObject? parent, LaneType[] laneTypes, WorldDirection worldDirection,
			string name = "", bool addTrafficLights = true, int distance = 10)
			: base(root, parent, worldDirection, name, distance)
		{
			LaneTypes = laneTypes;
			ContainsTrafficLights = addTrafficLights;

			if (addTrafficLights)
			{
				TrafficLights = new TrafficLights(root, this);
			}
		}

		public LaneType[] LaneTypes { get; set; }

		// TODO: Intersection Builder should assign here Traffic Lights.
		// TODO: Add conditional right green light in the future
		public TrafficLights? TrafficLights { get; }

		public bool ContainsTrafficLights { get; }

		public ICarGenerator? CarGenerator { get; set; }
	}
}
