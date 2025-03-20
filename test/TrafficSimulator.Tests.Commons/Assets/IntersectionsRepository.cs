using ErrorOr;
using FluentAssertions;
using MediatR;
using TrafficSimulator.Application.TrafficLights.Handlers;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.Cars;
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

		public static IntersectionSimulation FourDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(ISender mediator)
		{
			ErrorOr<Intersection> intersectionResult =
			IntersectionBuilder.Create("NormalFourStreets")
			.AddIntersectionCore()
			.AddLanesCollection(WorldDirection.North)
			.AddInboundLane(WorldDirection.North, LaneTypeHelper.StraightLeftAndRight())
			.AddOutboundLane(WorldDirection.North)
			.AddLanesCollection(WorldDirection.East)
			.AddInboundLane(WorldDirection.East, LaneTypeHelper.StraightLeftAndRight())
			.AddOutboundLane(WorldDirection.East)
			.AddLanesCollection(WorldDirection.South)
			.AddInboundLane(WorldDirection.South, LaneTypeHelper.StraightLeftAndRight())
			.AddOutboundLane(WorldDirection.South)
			.AddLanesCollection(WorldDirection.West)
			.AddInboundLane(WorldDirection.West, LaneTypeHelper.StraightLeftAndRight())
			.AddOutboundLane(WorldDirection.West)
			.Build();

			intersectionResult.IsError.Should().BeFalse();
			Intersection intersection = intersectionResult.Value;

			InboundLane northInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.North)!
				.InboundLanes!
				.First();

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

			ICarGenerator northLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator);
			northInboundLane.CarGenerator = northLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			ICarGenerator southLaneCarGenerator = new MultipleCarsGenerator(intersection, southInboundLane, mediator);
			southInboundLane.CarGenerator = southLaneCarGenerator;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, mediator);

			westInboundLane.CarGenerator = westLaneCarGenerator;
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.North));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("f004abcd-1d37-4289-928d-dd9798bf3007");

			return new IntersectionSimulation(intersection, id, "NormalFourStreetsSimulation");
		}

		public static IntersectionSimulation FourDirectional_Full(ISender mediator)
		{
			ErrorOr<Intersection> intersectionResult =
			IntersectionBuilder.Create("NormalFourStreets")
			.AddIntersectionCore(distance: 15)
			.AddLanesCollection(WorldDirection.North)
			.AddInboundLane(WorldDirection.North, LaneTypeHelper.StraightLeftAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.North, distance: 120)
			.AddLanesCollection(WorldDirection.East)
			.AddInboundLane(WorldDirection.East, LaneTypeHelper.StraightLeftAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.East, distance: 120)
			.AddLanesCollection(WorldDirection.South)
			.AddInboundLane(WorldDirection.South, LaneTypeHelper.StraightLeftAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.South, distance: 120)
			.AddLanesCollection(WorldDirection.West)
			.AddInboundLane(WorldDirection.West, LaneTypeHelper.StraightLeftAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.West, distance: 120)
			.Build();

			intersectionResult.IsError.Should().BeFalse();
			Intersection intersection = intersectionResult.Value;

			InboundLane northInboundLane = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.North)!
				.InboundLanes!
				.First();

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

			MultipleCarsGeneratorOptions multipleCarsGeneratorOptions = new()
			{
				AmountOfCarsToGenerate = 20,
				DelayBetweenCarGeneration = TimeSpan.FromMilliseconds(750),
				CarOptions = new CarOptions()
				{
					DistanceBetweenCars = 4,
					Length = 2,
					MoveVelocity = 30
				}
			};

			ICarGenerator northLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator, multipleCarsGeneratorOptions);
			northInboundLane.CarGenerator = northLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new MultipleCarsGenerator(intersection, eastInboundLane, mediator, multipleCarsGeneratorOptions);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			ICarGenerator southLaneCarGenerator = new MultipleCarsGenerator(intersection, southInboundLane, mediator, multipleCarsGeneratorOptions);
			southInboundLane.CarGenerator = southLaneCarGenerator;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, mediator, multipleCarsGeneratorOptions);

			westInboundLane.CarGenerator = westLaneCarGenerator;
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.North));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("f004accd-1d37-4289-928d-dd9798bf3007");

			IntersectionSimulationOptions intersectionSimulationOptions = new()
			{
				CarOptions = new CarOptions()
				{
					DistanceBetweenCars = 4,
					Length = 2,
					MoveVelocity = 30
				},
				StepLimit = 1200,
				StepTimespan = TimeSpan.FromMilliseconds(40),
				Timeout = TimeSpan.FromSeconds(15),
				TrafficLightHandlerType = TrafficLightHandlerTypes.Manual
			};

			return new IntersectionSimulation(intersection, id, "NormalFourStreetsSimulation")
			{
				Options = intersectionSimulationOptions
			};
		}
	}
}
