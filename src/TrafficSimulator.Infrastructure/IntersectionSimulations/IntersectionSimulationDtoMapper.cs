using ErrorOr;
using System.Text.Json;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Commons.Builders;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.IntersectionProperties;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DTOs;

namespace TrafficSimulator.Infrastructure.IntersectionSimulations
{
	public class IntersectionSimulationDtoMapper : ISimulationSetupMapper
	{
		private readonly CarGeneratorFactory _carGeneratorFactory;

		public IntersectionSimulationDtoMapper(CarGeneratorFactory carGeneratorFactory)
		{
			_carGeneratorFactory = carGeneratorFactory;
		}

		public ErrorOr<IntersectionSimulation> ToDomain(IntersectionSimulationDto intersectionSimulationDto)
		{
			IntersectionDto intersectionDto = intersectionSimulationDto.Intersection;

			IntersectionBuilder intersectionBuilder =
				IntersectionBuilder.Create(intersectionSimulationDto.Intersection.Name)
				.AddIntersectionCore(
					intersectionDto.IntersectionCore.Name,
					intersectionDto.IntersectionCore.Distance
					);

			foreach (LanesDto lanes in intersectionDto.LanesCollection)
			{
				intersectionBuilder.AddLanesCollection(lanes.WorldDirection, lanes.Name);

				foreach (OutboundLaneDto outboundLaneDto in lanes.OutboundLanes)
				{
					intersectionBuilder.AddOutboundLane(outboundLaneDto.WorldDirection, outboundLaneDto.Name, outboundLaneDto.Distance);
				}

				foreach (InboundLaneDto inboundLaneDto in lanes.InboundLanes)
				{
					intersectionBuilder.AddInboundLane(
						inboundLaneDto.WorldDirection, inboundLaneDto.LaneTypes, inboundLaneDto.Name,
						inboundLaneDto.ContainsTrafficLights, inboundLaneDto.Distance);
				}
			}

			ErrorOr<Intersection> intersectionResult = intersectionBuilder.Build();

			if (intersectionResult.IsError)
			{
				// TODO: Handle
				throw new NotImplementedException();
			}

			IntersectionSimulation intersectionSimulation = new(intersectionResult.Value, intersectionSimulationDto.Id, intersectionSimulationDto.Name)
			{
				Options = ToDomain(intersectionSimulationDto.Options),
			};

			Intersection intersection = intersectionSimulation.Intersection;

			intersectionDto.TrafficPhases.ForEach(trafficPhase =>
			{
				intersection.TrafficPhases.Add(new TrafficPhase(trafficPhase.Name, intersection)
				{
					TrafficLightsAssignments = trafficPhase.TrafficLightsAssignments.Select(light =>
					{
						InboundLane? inboundLane = intersection.GetObject<InboundLane>(light.InboundLaneName);
						LaneType laneType = inboundLane.LaneTypes.First(laneType => laneType == light.TurnPossibility.LaneType);

						return new TurnWithTrafficLight()
						{
							InboundLane = inboundLane,
							TrafficLightState = light.TrafficLightState,
							TurnPossibility = new TurnPossibility()
							{
								ContainsTrafficLights = inboundLane.ContainsTrafficLights,
								LaneType = laneType,
								TrafficLights = inboundLane.TrafficLights
							}
						};
					}).ToList()
				});
			});

			intersectionDto.LanesCollection.ForEach(lanes => lanes.InboundLanes.ForEach(lane =>
			{
				string fullName = $"{lane.ParentName}.{lane.Name}";
				InboundLane? inboundLane = intersection.GetObject<InboundLane>(fullName);

				ErrorOr<ICarGenerator?> carGeneratorResult = _carGeneratorFactory.Create(lane.CarGeneratorTypeName, lane.CarGeneratorOptions, intersection, inboundLane!);

				if (carGeneratorResult.IsError)
				{
					// TODO: Handle
					throw new Exception();
				}

				inboundLane.CarGenerator = carGeneratorResult.Value;
			}));

			return intersectionSimulation;
		}

		private IntersectionSimulationOptions ToDomain(IntersectionSimulationOptionsDto intersectionSimulationOptionsDto)
		{
			return new IntersectionSimulationOptions()
			{
				MinimalDistanceBetweenTheCars = intersectionSimulationOptionsDto.MinimalDistanceBetweenTheCars,
				StepLimit = intersectionSimulationOptionsDto.StepLimit,
				StepTimespan = TimeSpan.FromMilliseconds(intersectionSimulationOptionsDto.StepTimespanMs),
				Timeout = TimeSpan.FromMilliseconds(intersectionSimulationOptionsDto.TimeoutMs),
				TrafficLightHandlerType = intersectionSimulationOptionsDto.TrafficLightHandlerType
			};
		}

		public ErrorOr<IntersectionSimulationDto> ToDto(IntersectionSimulation intersectionSimulation)
		{
			return new IntersectionSimulationDto()
			{
				Options = ToDto(intersectionSimulation.Options),
				Intersection = ToDto(intersectionSimulation.Intersection),
				Id = intersectionSimulation.Id,
				Name = intersectionSimulation.Name,
			};
		}

		private IntersectionDto ToDto(Intersection intersection)
		{
			return new IntersectionDto()
			{
				Name = intersection.Name,
				IntersectionCore = ToDto(intersection.IntersectionCore),
				ParentName = string.Empty,
				TrafficPhases = ToDto(intersection.TrafficPhases),
				LanesCollection = ToDto(intersection.LanesCollection)
			};
		}

		private List<LanesDto> ToDto(List<Lanes> lanesCollection)
		{
			return lanesCollection.Select(ToDto).ToList();
		}

		private LanesDto ToDto(Lanes lanes)
		{
			return new LanesDto()
			{
				Name = lanes.Name,
				ParentName = lanes.Parent.FullName,
				WorldDirection = lanes.WorldDirection,
				InboundLanes = ToDto(lanes.InboundLanes),
				OutboundLanes = ToDto(lanes.OutboundLanes),
			};
		}

		private List<OutboundLaneDto> ToDto(List<OutboundLane>? outboundLanes)
		{
			return outboundLanes.Select(ToDto).ToList();
		}

		private OutboundLaneDto ToDto(OutboundLane lane)
		{
			return new OutboundLaneDto()
			{
				Distance = lane.Distance,
				Name = lane.Name,
				WorldDirection = lane.WorldDirection,
				ParentName = lane.Parent.FullName
			};
		}

		private List<InboundLaneDto> ToDto(List<InboundLane> inboundLanes)
		{
			return inboundLanes.Select(ToDto).ToList();
		}

		private InboundLaneDto ToDto(InboundLane lane)
		{
			string carGeneratorOptions = string.Empty;

			if (lane.CarGenerator is not null)
			{
				carGeneratorOptions = JsonSerializer.Serialize(
					lane.CarGenerator.Options,
					lane.CarGenerator.Options.GetType(),
					new JsonSerializerOptions { WriteIndented = true }
				);
			}
			else
			{
				carGeneratorOptions = JsonSerializer.Serialize(
					new CarGeneratorOptions(),
					new JsonSerializerOptions { WriteIndented = true }
				);
			}

			return new InboundLaneDto()
			{
				Distance = lane.Distance,
				Name = lane.Name,
				WorldDirection = lane.WorldDirection,
				ParentName = lane.Parent.FullName,
				LaneTypes = lane.LaneTypes,
				ContainsTrafficLights = lane.ContainsTrafficLights,
				CarGeneratorTypeName = lane.CarGenerator is null ? string.Empty : lane.CarGenerator.GetType().Name,
				CarGeneratorOptions = JsonSerializer.Deserialize<JsonElement>(carGeneratorOptions),
			};
		}


		private TurnPossibilityDto ToDto(TurnPossibility turn)
		{
			return new TurnPossibilityDto()
			{
				ContainsTrafficLights = turn.ContainsTrafficLights,
				LaneType = turn.LaneType,
			};
		}

		private List<TrafficPhaseDto> ToDto(List<TrafficPhase> trafficPhases)
		{
			return trafficPhases.Select(ToDto).ToList();
		}

		private TrafficPhaseDto ToDto(TrafficPhase trafficPhase)
		{
			return new TrafficPhaseDto()
			{
				Name = trafficPhase.Name,
				TrafficLightsAssignments = ToDto(trafficPhase.TrafficLightsAssignments),
			};
		}

		private List<TurnWithTrafficLightDto> ToDto(List<TurnWithTrafficLight> trafficLightsAssignments)
		{
			return trafficLightsAssignments.Select(ToDto).ToList();
		}

		private TurnWithTrafficLightDto ToDto(TurnWithTrafficLight trafficLightsAssignment)
		{
			return new TurnWithTrafficLightDto()
			{
				InboundLaneName = trafficLightsAssignment.InboundLane.FullName,
				TrafficLightState = trafficLightsAssignment.TrafficLightState,
				TurnPossibility = ToDto(trafficLightsAssignment.TurnPossibility),
			};
		}

		private IntersectionCoreDto ToDto(IntersectionCore? intersectionCore)
		{
			return new IntersectionCoreDto()
			{
				Distance = intersectionCore.Distance,
				Name = intersectionCore.Name,
				ParentName = intersectionCore.Parent.FullName
			};
		}

		private IntersectionSimulationOptionsDto ToDto(IntersectionSimulationOptions intersectionSimulationOptions)
		{
			return new IntersectionSimulationOptionsDto()
			{
				MinimalDistanceBetweenTheCars = intersectionSimulationOptions.MinimalDistanceBetweenTheCars,
				StepLimit = intersectionSimulationOptions.StepLimit,
				StepTimespanMs = intersectionSimulationOptions.StepTimespan.TotalMilliseconds,
				TimeoutMs = intersectionSimulationOptions.Timeout.TotalMilliseconds,
				TrafficLightHandlerType = intersectionSimulationOptions.TrafficLightHandlerType
			};
		}
	}
}
