using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Interfaces;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class InboundLane : OutboundLane, IEquatable<InboundLane>
	{
		// TODO: Pass information about the order number for the lane in the direction. (e.g. second (from left) on the West side of the Intersection)
		public InboundLane(
			Intersection root, IntersectionObject? parent, LaneType[] laneTypes, WorldDirection worldDirection,
			string name = "", bool addTrafficLights = true, int distance = 100)
			: base(root, parent, worldDirection, name, distance)
		{
			LaneTypes = laneTypes;
			ContainsTrafficLights = addTrafficLights;

			if (addTrafficLights)
			{
				TrafficLights = new TrafficLight(root, this);
			}

			if (string.IsNullOrEmpty(name))
			{
				DeterminePositionOfTheInboundLane(root, worldDirection);
				Name = string.Concat(Name, "_", PositionOnTheLane);
			}
		}

		public LaneType[] LaneTypes { get; set; }

		// TODO: Intersection Builder should assign here Traffic Lights.
		// TODO: Add conditional right green light in the future
		public TrafficLight? TrafficLights { get; }

		public bool ContainsTrafficLights { get; }

		public ICarGenerator? CarGenerator { get; set; }

		private void DeterminePositionOfTheInboundLane(Intersection root, WorldDirection worldDirection)
		{
			IEnumerable<InboundLane> lanesOnTheSameDirection = root.ObjectLookup
				.OfType<InboundLane>()
				.Where(lane => lane.WorldDirection == worldDirection);

			PositionOnTheLane = lanesOnTheSameDirection.Count();
		}

		public override bool Equals(object? obj)
		{
			return obj is InboundLane other && Equals(other);
		}

		public bool Equals(InboundLane? other)
		{
			if (other == null) return false;

			return base.Equals(other)
				&& ContainsTrafficLights == other.ContainsTrafficLights
				&& LaneTypes.SequenceEqual(other.LaneTypes);
		}
	}
}
