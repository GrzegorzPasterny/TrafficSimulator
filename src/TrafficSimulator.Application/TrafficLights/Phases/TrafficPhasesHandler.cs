using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.TrafficPhases
{
	public class TrafficPhasesHandler
	{
		private Intersection? _intersection;
		private TimeSpan _currentPhaseDuration;
		private TimeSpan _lightsChangeDuration = TimeSpan.FromMilliseconds(500);
		public bool AreLightsChanging => CurrentPhase != null && _currentPhaseDuration < _lightsChangeDuration;

		public TrafficPhasesHandler(Intersection intersection)
		{
			_intersection = intersection;
		}

		private TrafficPhasesHandler()
		{
		}

		internal List<TrafficPhase>? TrafficPhases => _intersection?.TrafficPhases;

		// Current phase must be assigned when starting the simulation
		public TrafficPhase? CurrentPhase { get; set; }

		public void LoadIntersection(Intersection intersection)
		{
			_intersection = intersection;
		}

		public UnitResult<Error> SetPhase(TrafficPhase trafficPhase, TimeSpan timeElapsed)
		{
			if (TrafficPhases is null)
			{
				return ApplicationErrors.IntersectionUninitialized();
			}

			UnitResult<Error> checkResult = HandlePhaseChangingConditions(trafficPhase, timeElapsed);

			if (checkResult.IsFailure)
			{
				return checkResult;
			}

			return SetLights(trafficPhase);
		}

		public UnitResult<Error> SetPhase(string trafficPhaseName, TimeSpan timeElapsed)
		{
			if (TrafficPhases is null)
			{
				return ApplicationErrors.IntersectionUninitialized();
			}

			TrafficPhase? trafficPhase = TrafficPhases.Find((p) => p.Name == trafficPhaseName);

			UnitResult<Error> checkResult = HandlePhaseChangingConditions(trafficPhase, timeElapsed);

			if (checkResult.IsFailure)
			{
				return checkResult;
			}

			return SetLights(trafficPhase);
		}

		private UnitResult<Error> HandlePhaseChangingConditions(TrafficPhase nextTrafficPhase, TimeSpan timeElapsed)
		{
			_currentPhaseDuration += timeElapsed;

			if (AreLightsChanging)
			{
				return ApplicationErrors.TrafficLightsChangeAttemptedTooSoon(
					CurrentPhase!.Name, (int)_currentPhaseDuration.TotalMicroseconds, nextTrafficPhase.Name);
			}

			return UnitResult.Success<Error>();
		}

		private UnitResult<Error> SetLights(TrafficPhase? nextTrafficPhase)
		{
			if (nextTrafficPhase is null)
			{
				// TODO Handle
				throw new NotImplementedException();
			}

			if (CurrentPhase == nextTrafficPhase)
			{
				return UnitResult.Success<Error>();
			}

			foreach (var turn in nextTrafficPhase.TrafficLightsAssignments)
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

			CurrentPhase = nextTrafficPhase;
			_currentPhaseDuration = TimeSpan.Zero;
			return UnitResult.Success<Error>();
		}
	}
}
