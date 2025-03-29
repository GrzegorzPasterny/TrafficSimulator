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

		public TrafficPhasesHandler()
		{
		}

		internal List<TrafficPhase>? TrafficPhases => _intersection?.TrafficPhases;

		// Current phase must be assigned when starting the simulation
		public TrafficPhase? CurrentPhase { get; set; }
		public TrafficPhase? PreviousPhase { get; set; }

		public void LoadIntersection(Intersection intersection)
		{
			_intersection = intersection;
			_currentPhaseDuration = TimeSpan.Zero;
			CurrentPhase = null;
			PreviousPhase = null;
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

			if (AreLightsChanging && nextTrafficPhase != CurrentPhase)
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
				ApplyLights();
			}
			else
			{
				PreviousPhase = CurrentPhase;
				CurrentPhase = nextTrafficPhase;
				_currentPhaseDuration = TimeSpan.Zero;

				ApplyLights();
			}

			return UnitResult.Success<Error>();
		}

		private void ApplyLights()
		{
			if (AreLightsChanging)
			{
				PreviousPhase?.ApplyChange();
				CurrentPhase!.ApplyChange();
			}
			else
			{
				CurrentPhase!.Apply();
			}
		}
	}
}
