using ErrorOr;
using TrafficSimulator.Domain.Commons.Interfaces;
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

		public IntersectionBuilder AddIntersectionCore(int distance = 10)
		{
			_intersection.IntersectionCore = new IntersectionCore(_intersection, _intersection, distance);

			return this;
		}

		public IntersectionBuilder AddLanesCollection(WorldDirection worldDirection)
		{
			string name = $"{nameof(Intersection)}.{nameof(Lanes)}.{worldDirection}";

			Lanes lanes = new Lanes(_intersection, _intersection, worldDirection);

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
				InboundLane inboundLane = new(_intersection, lanesCollection, LaneTypeHelper.Straight());

				lanesCollection.InboundLanes!.Add(inboundLane);
			}
			else
			{
				OutboundLane outboundLane = new(_intersection, lanesCollection);

				lanesCollection.OutboundLanes!.Add(outboundLane);
			}

			return this;
		}

		public IntersectionBuilder AddCarGenerator(ICarGenerator carGenerator, WorldDirection worldDirection, int laneNumber)
		{
			Lanes? lanes = _intersection.LanesCollection.Find(l => l.WorldDirection == worldDirection);

			if (lanes is null)
			{
				// TODO Handle
			}

			if (lanes!.InboundLanes is null || lanes!.InboundLanes!.Count < --laneNumber)
			{
				// TODO Handle
			}

			InboundLane inboundLane = lanes!.InboundLanes![laneNumber];

			inboundLane.CarGenerator = carGenerator;

			return this;
		}

		public ErrorOr<Intersection> Build()
		{
			// TODO: Check if intersection is not missing required elements
			return _intersection;
		}
	}
}
