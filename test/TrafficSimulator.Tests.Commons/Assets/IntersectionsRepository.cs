using ErrorOr;
using FluentAssertions;
using MediatR;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Handlers.CarGenerators;
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

				Guid id = Guid.Parse("5ce2fb45-c62b-4b92-88ef-456ed1dbe66e");

				return new IntersectionSimulation(intersection, id, "ZebraCrossingSimulation");
			}
		}

		public static IntersectionSimulation ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators(ISender mediator)
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

			InboundLane westInboundLane = intersection.LanesCollection!
			.Find(l => l.WorldDirection == WorldDirection.West)!
			.InboundLanes!
			.First();

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			ICarGenerator westLaneCarGenerator = new SingleCarGenerator(intersection, westInboundLane, mediator);
			westInboundLane.CarGenerator = westLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new SingleCarGenerator(intersection, eastInboundLane, mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			intersection.TrafficPhases.Add(TrafficPhasesRespository.AllLightsGreen(intersection));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.AllLightsRed(intersection));

			Guid id = Guid.Parse("6ce2fb45-c62b-4b92-88ef-456ed1dbe66e");

			return new IntersectionSimulation(intersection, id, "ZebraCrossingSimulation");
		}

		public static IntersectionSimulation ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create("Fork")
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

				Guid id = Guid.Parse("641c2b72-da21-424e-8b43-3b339415013b");

				return new IntersectionSimulation(intersection, id, "ForkSimulation");
			}
		}

		public static IntersectionSimulation ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLightsWithMultipleCarGenerators(ISender mediator)
		{
			ErrorOr<Intersection> intersectionResult =
			IntersectionBuilder.Create("Fork")
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

			InboundLane westInboundLane = intersection.LanesCollection!
			.Find(l => l.WorldDirection == WorldDirection.West)!
			.InboundLanes!
			.First();

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, mediator);
			westInboundLane.CarGenerator = westLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("741c2b72-da21-424e-8b43-3b339415013b");

			return new IntersectionSimulation(intersection, id, "ForkSimulation");
		}

		public static IntersectionSimulation ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights
		{
			get
			{
				ErrorOr<Intersection> intersectionResult =
				IntersectionBuilder.Create("NormalThreeStreets")
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

				Guid id = Guid.Parse("0dac5dbe-1d37-4289-928d-dd9798bf3007");

				return new IntersectionSimulation(intersection, id, "NormalThreeStreetsSimulation");
			}
		}

		public static IntersectionSimulation ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(ISender mediator)
		{
			ErrorOr<Intersection> intersectionResult =
			IntersectionBuilder.Create("NormalThreeStreets")
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

			InboundLane eastInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!
				.First();

			InboundLane southInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.South)!
				.InboundLanes!
				.First();

			InboundLane westInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!
				.First();

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			ICarGenerator southLaneCarGenerator = new MultipleCarsGenerator(intersection, southInboundLane, mediator);
			southInboundLane.CarGenerator = southLaneCarGenerator;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, mediator);

			westInboundLane.CarGenerator = westLaneCarGenerator;
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("1dac5dbe-1d37-4289-928d-dd9798bf3007");

			return new IntersectionSimulation(intersection, id, "NormalThreeStreetsSimulation");
		}
	}
}
