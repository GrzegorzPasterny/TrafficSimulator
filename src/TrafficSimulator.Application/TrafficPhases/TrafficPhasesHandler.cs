using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.TrafficPhases
{
	public class TrafficPhasesHandler
	{
		private readonly Intersection _intersection;

		public TrafficPhasesHandler(Intersection intersection)
		{
			_intersection = intersection;
		}

		public List<TrafficPhase> TrafficPhases => _intersection.TrafficPhases;

		// Current phase must be assigned when starting the simulation
		public TrafficPhase? CurrentPhase { get; set; }

		public UnitResult<Error> SetPhase(TrafficPhase trafficPhase)
		{
			return SetLights(trafficPhase);
		}

		public UnitResult<Error> SetPhase(string trafficPhaseName)
		{
			TrafficPhase? trafficPhase = TrafficPhases.Find((p) => p.Name == trafficPhaseName);

			return SetLights(trafficPhase);
		}

		private UnitResult<Error> SetLights(TrafficPhase? trafficPhase)
		{
			if (trafficPhase is null)
			{
				// TODO Handle
				throw new NotImplementedException();
			}

			foreach (var turn in trafficPhase.TrafficLightsAssignments)
			{
				// TODO: Handle all of the cases below
				switch (turn.TrafficLightState)
				{
					case TrafficLightState.Off:
						break;
					case TrafficLightState.Green:
						turn.TurnPossibility.TrafficLights!.SwitchToGreen();
						break;
					case TrafficLightState.Orange:
						break;
					case TrafficLightState.Red:
						turn.TurnPossibility.TrafficLights!.SwitchToRed();
						break;
					case TrafficLightState.ConditionalRightGreen:
						break;
					default:
						break;
				}
			}

			CurrentPhase = trafficPhase;
			return UnitResult.Success<Error>();
		}
	}
}
