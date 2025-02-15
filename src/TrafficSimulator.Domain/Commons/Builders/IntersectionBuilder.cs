using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons.Builders
{
	public class IntersectionBuilder
	{
		private Intersection _intersection;

		private IntersectionBuilder(Intersection intersection)
		{
			_intersection = intersection;
		}

		public static IntersectionBuilder Create(string intersectionName = "intersection")
		{
			Intersection intersection = new Intersection(intersectionName);

			IntersectionBuilder builder = new IntersectionBuilder(intersection);

			return builder;
		}

		public IntersectionBuilder AddLanesCollection(WorldDirection worldDirection)
		{
			string name = $"{nameof(Intersection)}.{nameof(Lanes)}.{worldDirection}";

			Lanes lanes = new Lanes(_intersection, worldDirection);

			// TODO: Check if there is already lanes object with the same world direction
			// TODO: Shuold throw an exception?

			_intersection.LanesCollection.Add(lanes);

			return this;
		}

		public IntersectionBuilder AddLane(WorldDirection worldDirection, bool isInbound)
		{
			Lanes lanesCollection = _intersection.LanesCollection.Single(l => l.WorldDirection == worldDirection);

			if (isInbound)
			{
				InboundLane inboundLane = new(_intersection, LaneTypeHelper.Straight());

				lanesCollection.InboundLanes.Add(inboundLane);
			}
			else
			{
				OutboundLane outboundLane = new(_intersection, LaneTypeHelper.Straight());
			}

			return this;
		}

		public ErrorOr<Intersection> Build()
		{
			// TODO: Check if intersection is not missing required elements
			return _intersection;
		}
	}
}
