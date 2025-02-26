using ErrorOr;
using FluentAssertions;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Tests.Commons.Assets
{
	public static class IntersectionsRepository
	{
		public static IntersectionSimulation ZebraCrossingOnOneLaneRoadEastWest
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

				intersection.TrafficPhases.Add(TrafficPhasesRespository.AllLightsGreen(intersection));
				intersection.TrafficPhases.Add(TrafficPhasesRespository.AllLightsRed(intersection));

				return new IntersectionSimulation(intersection);
			}
		}

		public static IntersectionSimulation ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights
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
				intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
				intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

				return new IntersectionSimulation(intersection);
			}
		}

		public static IntersectionSimulation ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights
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

				intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
				intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
				intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

				return new IntersectionSimulation(intersection);
			}
		}
	}
}
