using ErrorOr;
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
		public ErrorOr<IntersectionSimulation> ToDomain(IntersectionSimulationDto intersectionSimulationDto)
		{
			throw new NotImplementedException();
		}

		public ErrorOr<IntersectionSimulationDto> ToDto(IntersectionSimulation intersectionSimulation)
		{
			return new IntersectionSimulationDto()
			{
				Options = ToDto(intersectionSimulation.Options),
				Intersection = ToDto(intersectionSimulation.Intersection)
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
			return new InboundLaneDto()
			{
				Distance = lane.Distance,
				Name = lane.Name,
				WorldDirection = lane.WorldDirection,
				ParentName = lane.Parent.FullName,
				TurnPossibilities = ToDto(lane.TurnPossibilities),
				CarGeneratorTypeName = lane.CarGenerator is null ? string.Empty : lane.CarGenerator.GetType().Name
			};
		}

		private List<TurnPossibilityDto> ToDto(List<TurnPossibility> turnPossibilities)
		{
			return turnPossibilities.Select(ToDto).ToList();
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
			};
		}
	}
}
