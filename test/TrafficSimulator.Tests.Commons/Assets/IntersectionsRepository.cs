using ErrorOr;
using FluentAssertions;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Tests.Commons.Assets
{
	public static class IntersectionsRepository
	{
		public static Intersection ZebraCrossingOnOneLaneRoadEastWest
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create("ZebraCrossing")
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddInboundLane(WorldDirection.East, LaneTypeHelper.Straight())
				.AddOutboundLane(WorldDirection.East)
				.AddLanesCollection(WorldDirection.West)
				.AddInboundLane(WorldDirection.West, LaneTypeHelper.Straight())
				.AddOutboundLane(WorldDirection.West)
				.Build();

				intersectionResult.IsError.Should().BeFalse();

				Intersection intersection = intersectionResult.Value;

				return intersection;
			}
		}

		public static Intersection ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create()
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddInboundLane(WorldDirection.East, LaneTypeHelper.Right())
				.AddLanesCollection(WorldDirection.West)
				.AddInboundLane(WorldDirection.West, LaneTypeHelper.Left())
				.AddLanesCollection(WorldDirection.North)
				.AddOutboundLane(WorldDirection.North)
				.Build();

				intersectionResult.IsError.Should().BeFalse();

				Intersection intersection = intersectionResult.Value;

				return intersection;
			}
		}

		public static Intersection ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create()
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddInboundLane(WorldDirection.East, LaneTypeHelper.StraightAndLeft())
				.AddOutboundLane(WorldDirection.East)
				.AddLanesCollection(WorldDirection.South)
				.AddInboundLane(WorldDirection.South, LaneTypeHelper.LeftAndRight())
				.AddOutboundLane(WorldDirection.South)
				.AddLanesCollection(WorldDirection.West)
				.AddInboundLane(WorldDirection.West, LaneTypeHelper.StraightAndRight())
				.AddOutboundLane(WorldDirection.West)
				.Build();

				intersectionResult.IsError.Should().BeFalse();

				Intersection intersection = intersectionResult.Value;

				return intersection;
			}
		}
	}
}
