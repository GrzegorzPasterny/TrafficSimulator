using ErrorOr;
using FluentAssertions;
using MediatR;
using TrafficSimulator.Application.TrafficLights.Handlers;
using TrafficSimulator.Application.UnitTests.Commons;
using TrafficSimulator.Domain.CarGenerators;
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
			IntersectionBuilder.Create("4StreetsFull")
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

			CarOptions carOptions = new CarOptions()
			{
				DistanceBetweenCars = 4,
				Length = 2,
				MoveVelocity = 30
			};

			MultipleCarsGeneratorOptions multipleCarsGeneratorOptions = new()
			{
				AmountOfCarsToGenerate = 25,
				DelayBetweenCarGeneration = TimeSpan.FromMilliseconds(750),
				CarOptions = carOptions
			};

			RandomCarsGeneratorOptions randomCarsGeneratorOptions = new()
			{
				AmountOfCarsToGenerate = 20,
				BaseRate = 1,
				CarOptions = carOptions
			};

			WaveCarsGeneratorOptions waveCarsGeneratorOptions = new()
			{
				BaseRate = 1,
				AmountOfCarsToGenerate = 20,
				WaveAmplitude = 30,
				WavePeriodHz = 4,
				CarOptions = carOptions
			};

			ICarGenerator northLaneCarGenerator = new MultipleCarsGenerator(intersection, northInboundLane, mediator, multipleCarsGeneratorOptions);
			northInboundLane.CarGenerator = northLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator = new RandomCarsGenerator(intersection, eastInboundLane, mediator, randomCarsGeneratorOptions);
			eastInboundLane.CarGenerator = eastLaneCarGenerator;

			ICarGenerator southLaneCarGenerator = new WaveCarsGenerator(intersection, southInboundLane, mediator, waveCarsGeneratorOptions);
			southInboundLane.CarGenerator = southLaneCarGenerator;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLane, mediator, multipleCarsGeneratorOptions);

			westInboundLane.CarGenerator = westLaneCarGenerator;
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.North));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("1000ffff-1d37-4289-928d-dd9798bf3007");

			IntersectionSimulationOptions intersectionSimulationOptions = new()
			{
				StepLimit = 1200,
				StepTimespan = TimeSpan.FromMilliseconds(40),
				Timeout = TimeSpan.FromSeconds(15),
				TrafficLightHandlerType = TrafficLightHandlerTypes.Manual
			};

			return new IntersectionSimulation(intersection, id, "4StreetsFull")
			{
				Options = intersectionSimulationOptions
			};
		}

		public static IntersectionSimulation FourDirectional_2Lanes_Full(ISender mediator)
		{
			ErrorOr<Intersection> intersectionResult =
			IntersectionBuilder.Create("4StreetsFull")
			.AddIntersectionCore(distance: 15)
			.AddLanesCollection(WorldDirection.North)
			.AddInboundLane(WorldDirection.North, LaneTypeHelper.StraightAndLeft(), distance: 120)
			.AddInboundLane(WorldDirection.North, LaneTypeHelper.StraightAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.North, distance: 120)
			.AddOutboundLane(WorldDirection.North, distance: 120)
			.AddLanesCollection(WorldDirection.East)
			.AddInboundLane(WorldDirection.East, LaneTypeHelper.StraightAndLeft(), distance: 120)
			.AddInboundLane(WorldDirection.East, LaneTypeHelper.StraightAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.East, distance: 120)
			.AddOutboundLane(WorldDirection.East, distance: 120)
			.AddLanesCollection(WorldDirection.South)
			.AddInboundLane(WorldDirection.South, LaneTypeHelper.StraightAndLeft(), distance: 120)
			.AddInboundLane(WorldDirection.South, LaneTypeHelper.StraightAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.South, distance: 120)
			.AddOutboundLane(WorldDirection.South, distance: 120)
			.AddLanesCollection(WorldDirection.West)
			.AddInboundLane(WorldDirection.West, LaneTypeHelper.StraightAndLeft(), distance: 120)
			.AddInboundLane(WorldDirection.West, LaneTypeHelper.StraightAndRight(), distance: 120)
			.AddOutboundLane(WorldDirection.West, distance: 120)
			.AddOutboundLane(WorldDirection.West, distance: 120)
			.Build();

			intersectionResult.IsError.Should().BeFalse();
			Intersection intersection = intersectionResult.Value;

			InboundLane[] northInboundLanes = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.North)!
				.InboundLanes!.ToArray();

			InboundLane[] eastInboundLanes = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.East)!
				.InboundLanes!.ToArray();

			InboundLane[] southInboundLanes = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.South)!
				.InboundLanes!.ToArray();

			InboundLane[] westInboundLanes = intersection.LanesCollection!
				.Find(l => l.WorldDirection == WorldDirection.West)!
				.InboundLanes!.ToArray();

			CarOptions carOptions = new CarOptions()
			{
				DistanceBetweenCars = 4,
				Length = 2,
				MoveVelocity = 30
			};

			MultipleCarsGeneratorOptions multipleCarsGeneratorOptions = new()
			{
				AmountOfCarsToGenerate = 25,
				DelayBetweenCarGeneration = TimeSpan.FromMilliseconds(750),
				CarOptions = carOptions
			};

			RandomCarsGeneratorOptions randomCarsGeneratorOptions = new()
			{
				AmountOfCarsToGenerate = 20,
				BaseRate = 1,
				CarOptions = carOptions
			};

			WaveCarsGeneratorOptions waveCarsGeneratorOptions = new()
			{
				BaseRate = 1,
				AmountOfCarsToGenerate = 20,
				WaveAmplitude = 30,
				WavePeriodHz = 4,
				CarOptions = carOptions
			};

			ICarGenerator northLaneCarGenerator = new MultipleCarsGenerator(intersection, northInboundLanes[0], mediator, multipleCarsGeneratorOptions);
			northInboundLanes[0].CarGenerator = northLaneCarGenerator;

			ICarGenerator northLaneCarGenerator2 = new RandomCarsGenerator(intersection, northInboundLanes[1], mediator, randomCarsGeneratorOptions);
			northInboundLanes[1].CarGenerator = northLaneCarGenerator2;

			ICarGenerator eastLaneCarGenerator = new RandomCarsGenerator(intersection, eastInboundLanes[0], mediator, randomCarsGeneratorOptions);
			eastInboundLanes[0].CarGenerator = eastLaneCarGenerator;

			ICarGenerator eastLaneCarGenerator2 = new WaveCarsGenerator(intersection, eastInboundLanes[1], mediator, waveCarsGeneratorOptions);
			eastInboundLanes[1].CarGenerator = eastLaneCarGenerator2;

			ICarGenerator southLaneCarGenerator = new WaveCarsGenerator(intersection, southInboundLanes[0], mediator, waveCarsGeneratorOptions);
			southInboundLanes[0].CarGenerator = southLaneCarGenerator;

			ICarGenerator southLaneCarGenerator2 = new WaveCarsGenerator(intersection, southInboundLanes[1], mediator, waveCarsGeneratorOptions);
			southInboundLanes[1].CarGenerator = southLaneCarGenerator2;

			ICarGenerator westLaneCarGenerator = new MultipleCarsGenerator(intersection, westInboundLanes[0], mediator, multipleCarsGeneratorOptions);
			westInboundLanes[0].CarGenerator = westLaneCarGenerator;

			ICarGenerator westLaneCarGenerator2 = new RandomCarsGenerator(intersection, westInboundLanes[1], mediator, randomCarsGeneratorOptions);
			westInboundLanes[1].CarGenerator = westLaneCarGenerator2;

			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.North));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.East));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.South));
			intersection.TrafficPhases.Add(TrafficPhasesRespository.GreenForOneDirection(intersection, WorldDirection.West));

			Guid id = Guid.Parse("2000ffff-1d37-4289-928d-dd9798bf3007");

			IntersectionSimulationOptions intersectionSimulationOptions = new()
			{
				StepLimit = 2000,
				StepTimespan = TimeSpan.FromMilliseconds(40),
				Timeout = TimeSpan.FromSeconds(60),
				TrafficLightHandlerType = TrafficLightHandlerTypes.Dynamic
			};

			return new IntersectionSimulation(intersection, id, "4StreetsFull2Lanes")
			{
				Options = intersectionSimulationOptions
			};
		}
	}
}
