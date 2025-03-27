using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Extensions;
using TrafficSimulator.Application.Commons.Helpers;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	/// <summary>
	/// Changes Traffic Lights phases one by one in some time interval
	/// </summary>
	public class SimpleSequentialTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly ILogger<SimpleSequentialTrafficLightsHandler> _logger;
		private CircularList<TrafficPhase>? _circularListForTrafficPhases;

		public TimeSpan CurrentPhaseTime { get; private set; }

		// Options
		public TimeSpan TimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleSequentialTrafficLightsHandler(TrafficPhasesHandler trafficPhasesHandler, ILogger<SimpleSequentialTrafficLightsHandler> logger)
		{
			_trafficPhasesHandler = trafficPhasesHandler;
			_logger = logger;

			if (trafficPhasesHandler.TrafficPhases is not null)
			{
				_circularListForTrafficPhases = new CircularList<TrafficPhase>(trafficPhasesHandler.TrafficPhases);
				_trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current, TimeSpan.Zero);

				_logger.LogTrace("Traffic Lights initial phase set [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
			}
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			if (_circularListForTrafficPhases is null)
			{
				return Task.FromResult(UnitResult.Failure(ApplicationErrors.IntersectionUninitialized()));
			}

			CurrentPhaseTime = CurrentPhaseTime.Add(timeElapsed);

			if (CurrentPhaseTime >= TimeForOnePhase)
			{
				_circularListForTrafficPhases.MoveNext();
				ChangeTrafficLightsPhase(timeElapsed);
			}

			return Task.FromResult(UnitResult.Success<Error>());
		}

		private void ChangeTrafficLightsPhase(TimeSpan timeElapsed)
		{
			// TODO: Handle
			_ = _trafficPhasesHandler.SetPhase(_circularListForTrafficPhases.Current, timeElapsed);
			CurrentPhaseTime = TimeSpan.Zero;

			_logger.LogTrace("Traffic Lights phase changed [TrafficLightsPhase = {TrafficLightsPhase}]", _trafficPhasesHandler.CurrentPhase);
		}

		public void LoadIntersection(Intersection intersection)
		{
			_trafficPhasesHandler.LoadIntersection(intersection);
			_circularListForTrafficPhases = new CircularList<TrafficPhase>(intersection.TrafficPhases);

			ChangeTrafficLightsPhase(TimeSpan.Zero);
		}

		public TrafficPhase GetCurrentTrafficPhase()
		{
			if (_circularListForTrafficPhases is null)
			{
				// TODO: Handle
				throw new NotImplementedException();
			}

			return _circularListForTrafficPhases.Current;
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			if (_circularListForTrafficPhases is null)
			{
				// TODO: Handle
				throw new NotImplementedException();
			}

			bool wasSet = _circularListForTrafficPhases.Set(trafficPhaseName, trafficPhase => trafficPhase.Name);

			if (wasSet == false)
			{
				// TODO: Handle
				throw new ArgumentOutOfRangeException();
			}

			ChangeTrafficLightsPhase(TimeSpan.Zero);
			return UnitResult.Success<Error>();
		}
	}
}
