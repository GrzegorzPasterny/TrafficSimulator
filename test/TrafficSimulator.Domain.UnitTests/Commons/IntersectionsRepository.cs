using ErrorOr;
using FluentAssertions;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.UnitTests.Commons
{
	public static class IntersectionsRepository
	{
		public static Intersection ZebraCrossingOnOneLaneRoadEastWest
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create()
				.AddIntersectionCore()
				.AddLanesCollection(WorldDirection.East)
				.AddLane(WorldDirection.East, true)
				.AddLane(WorldDirection.East, false)
				.AddLanesCollection(WorldDirection.West)
				.AddLane(WorldDirection.West, true)
				.AddLane(WorldDirection.West, false)
				.Build();

				intersectionResult.IsError.Should().BeFalse();

				Intersection intersection = intersectionResult.Value;

				return intersection;
			}
		}
	}
}
