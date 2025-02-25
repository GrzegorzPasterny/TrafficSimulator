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
			return lanesCollection.Select(lanes => ToDto(lanes)).ToList();
		}

		private LanesDto ToDto(Lanes lanes)
		{
			return new LanesDto()
			{
				Name = lanes.Name,
				ParentName = lanes.Parent.Name,
				WorldDirection = lanes.WorldDirection,
				InboundLanes = ToDto(lanes.InboundLanes),
				OutboundLanes = ToDto(lanes.OutboundLanes),
			};
		}

		private List<OutboundLaneDto> ToDto(List<OutboundLane>? outboundLanes)
		{
			return outboundLanes.Select(lane => ToDto(lane)).ToList();
		}

		private OutboundLaneDto ToDto(OutboundLane lane)
		{
			return new OutboundLaneDto()
			{
				Distance = lane.Distance,
				Name = lane.Name,
				WorldDirection = lane.WorldDirection,
				ParentName = lane.Parent.Name
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
				ParentName = lane.Parent.Name,
				TurnPossibilities = ToDto(lane.TurnPossibilities),
				CarGeneratorTypeName = lane.CarGenerator.GetType().Name
			};
		}

		private List<TurnPossibilityDto> ToDto(List<TurnPossibility> turnPossibilities)
		{
			return turnPossibilities.Select(turn => ToDto(turn)).ToList();
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
			return trafficPhases.Select(trafficPhase => ToDto(trafficPhase)).ToList();
		}

		private TrafficPhaseDto ToDto(TrafficPhase trafficPhase)
		{
			return new TrafficPhaseDto()
			{
				Name = trafficPhase.Name,
				TrafficLightsAssignments = trafficPhase.TrafficLightsAssignments,
			};
		}

		private IntersectionCoreDto ToDto(IntersectionCore? intersectionCore)
		{
			return new IntersectionCoreDto()
			{
				Distance = intersectionCore.Distance,
				Name = intersectionCore.Name,
				ParentName = intersectionCore.Parent.Name
			};
		}

		private IntersectionSimulationOptionsDto ToDto(IntersectionSimulationOptions intersectionSimulationOptions)
		{
			return new IntersectionSimulationOptionsDto()
			{
				MinimalDistanceBetweenTheCars = intersectionSimulationOptions.MinimalDistanceBetweenTheCars,
				StepLimit = intersectionSimulationOptions.StepLimit,
				StepTimespan = intersectionSimulationOptions.StepTimespan,
				Timeout = intersectionSimulationOptions.Timeout,
			};
		}
	}
}
