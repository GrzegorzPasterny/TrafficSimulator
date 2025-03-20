using System.Text.Json;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Handlers.CarGenerators;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Infrastructure.DTOs;

namespace TrafficSimulator.Infrastructure.UnitTests.Assets
{
	public static class IntersectionSimulationDtosRepository
	{
		public static IntersectionSimulationDto ZebraCrossingOnOneLaneRoadEastWest
		{
			get
			{
				return new IntersectionSimulationDto()
				{
					Id = Guid.Parse("5ce2fb45-c62b-4b92-88ef-456ed1dbe66e"),
					Name = "ZebraCrossingSimulation",
					Options = new()
					{
						CarOptions = new CarOptions()
						{
							DistanceBetweenCars = 1,
							Length = 2,
							MoveVelocity = 50
						},
						StepLimit = 1000,
						StepTimespanMs = 100,
						TimeoutMs = 5000
					},
					Intersection = new()
					{
						Name = "ZebraCrossing",
						ParentName = string.Empty,
						IntersectionCore = new()
						{
							Distance = 10,
							Name = "IntersectionCore",
							ParentName = ".ZebraCrossing"
						},
						LanesCollection = [
							new LanesDto()
							{
								Name = "Lanes.West",
								WorldDirection = WorldDirection.West,
								ParentName = ".ZebraCrossing",
								InboundLanes = [
									new InboundLaneDto()
									{
										Name = "InboundLane",
										ParentName = ".ZebraCrossing.Lanes.West",
										Distance = 10,
										WorldDirection = WorldDirection.West,
										LaneTypes = [
											LaneType.Straight
										],
										ContainsTrafficLights = true,
										CarGeneratorTypeName = "",
										CarGeneratorOptions = JsonSerializer.Deserialize<JsonElement>(
											JsonSerializer.Serialize(
												new CarGeneratorOptions(),
												new JsonSerializerOptions() { WriteIndented = true }))
									}
								],
								OutboundLanes = [
									new OutboundLaneDto()
									{
										Distance = 10,
										Name = "OutboundLane",
										ParentName = ".ZebraCrossing.Lanes.West",
										WorldDirection = WorldDirection.West
									}
								]
							},
							new LanesDto()
							{
								Name = "Lanes.East",
								WorldDirection = WorldDirection.East,
								ParentName = ".ZebraCrossing",
								InboundLanes = [
									new InboundLaneDto()
									{
										Name = "InboundLane",
										ParentName = ".ZebraCrossing.Lanes.East",
										Distance = 10,
										WorldDirection = WorldDirection.East,
										LaneTypes = [
											LaneType.Straight
										],
										ContainsTrafficLights = true,
										CarGeneratorTypeName = "",
										CarGeneratorOptions = JsonSerializer.Deserialize<JsonElement>(
											JsonSerializer.Serialize(
												new CarGeneratorOptions(),
												new JsonSerializerOptions() { WriteIndented = true }))
									}
								],
								OutboundLanes = [
									new OutboundLaneDto()
									{
										Distance = 10,
										Name = "OutboundLane",
										ParentName = ".ZebraCrossing.Lanes.East",
										WorldDirection = WorldDirection.East
									}
								]
							}
						],
						TrafficPhases = [
							new TrafficPhaseDto()
							{
								Name = "AllGreen",
								TrafficLightsAssignments = [
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.East.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Green,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.West.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Green,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
								]
							},
							new TrafficPhaseDto()
							{
								Name = "AllRed",
								TrafficLightsAssignments = [
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.East.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Red,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.West.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Red,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
								]
							}
						]
					}
				};
			}
		}
		public static IntersectionSimulationDto ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators
		{
			get
			{
				return new IntersectionSimulationDto()
				{
					Id = Guid.Parse("6ce2fb45-c62b-4b92-88ef-456ed1dbe66e"),
					Name = "ZebraCrossingSimulation",
					Options = new()
					{
						CarOptions = new CarOptions(),
						StepLimit = 1000,
						StepTimespanMs = 40,
						TimeoutMs = 10000
					},
					Intersection = new()
					{
						Name = "ZebraCrossing",
						ParentName = string.Empty,
						IntersectionCore = new()
						{
							Distance = 10,
							Name = "IntersectionCore",
							ParentName = ".ZebraCrossing"
						},
						LanesCollection = [
							new LanesDto()
							{
								Name = "Lanes.West",
								WorldDirection = WorldDirection.West,
								ParentName = ".ZebraCrossing",
								InboundLanes = [
									new InboundLaneDto()
									{
										Name = "InboundLane",
										ParentName = ".ZebraCrossing.Lanes.West",
										Distance = 100,
										WorldDirection = WorldDirection.West,
										LaneTypes = [
											LaneType.Straight
										],
										ContainsTrafficLights = true,
										CarGeneratorTypeName = "SingleCarGenerator",
										CarGeneratorOptions = JsonSerializer.Deserialize<JsonElement>(
											JsonSerializer.Serialize(
												new SingleCarGeneratorOptions() { DelayForGeneratingTheCar = TimeSpan.FromMilliseconds(200) },
												new JsonSerializerOptions() { WriteIndented = true }))
									}
								],
								OutboundLanes = [
									new OutboundLaneDto()
									{
										Distance = 100,
										Name = "OutboundLane",
										ParentName = ".ZebraCrossing.Lanes.West",
										WorldDirection = WorldDirection.West
									}
								]
							},
							new LanesDto()
							{
								Name = "Lanes.East",
								WorldDirection = WorldDirection.East,
								ParentName = ".ZebraCrossing",
								InboundLanes = [
									new InboundLaneDto()
									{
										Name = "InboundLane",
										ParentName = ".ZebraCrossing.Lanes.East",
										Distance = 100,
										WorldDirection = WorldDirection.East,
										LaneTypes = [
											LaneType.Straight
										],
										ContainsTrafficLights = true,
										CarGeneratorTypeName = "SingleCarGenerator",
										CarGeneratorOptions = JsonSerializer.Deserialize<JsonElement>(
											JsonSerializer.Serialize(
												new SingleCarGeneratorOptions() { DelayForGeneratingTheCar = TimeSpan.FromMilliseconds(200) },
												new JsonSerializerOptions() { WriteIndented = true }))
									}
								],
								OutboundLanes = [
									new OutboundLaneDto()
									{
										Distance = 100,
										Name = "OutboundLane",
										ParentName = ".ZebraCrossing.Lanes.East",
										WorldDirection = WorldDirection.East
									}
								]
							}
						],
						TrafficPhases = [
							new TrafficPhaseDto()
							{
								Name = "AllGreen",
								TrafficLightsAssignments = [
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.East.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Green,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.West.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Green,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
								]
							},
							new TrafficPhaseDto()
							{
								Name = "AllRed",
								TrafficLightsAssignments = [
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.East.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Red,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
									new TurnWithTrafficLightDto()
									{
										InboundLaneName = ".ZebraCrossing.Lanes.West.InboundLane",
										TrafficLightState = Domain.Models.Lights.TrafficLightState.Red,
										TurnPossibility = new TurnPossibilityDto()
										{
											ContainsTrafficLights = true,
											LaneType = LaneType.Straight
										}
									},
								]
							}
						]
					}
				};
			}
		}
	}
}
