using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using SharpNeat;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.Nest
{
	public class NestTrafficLightsHandler : ITrafficLightsHandler
	{
		internal readonly ISender _sender;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly ILogger<NestTrafficLightsHandler> _logger;
		private IBlackBox<double> _blackBox;

		public TimeSpan CurrentPhaseTime { get; internal set; } = TimeSpan.Zero;
		public TimeSpan MinimalTimeForOnePhase { get; set; } = TimeSpan.FromSeconds(2);

		public NestTrafficLightsHandler(
			ISender sender, TrafficPhasesHandler trafficPhasesHandler, ILogger<NestTrafficLightsHandler> logger)
		{
			_sender = sender;
			_trafficPhasesHandler = trafficPhasesHandler;
			_logger = logger;
		}

		public TrafficPhase? GetCurrentTrafficPhase()
		{
			return _trafficPhasesHandler.CurrentPhase;
		}

		public void LoadIntersection(Intersection intersection)
		{
			_trafficPhasesHandler.LoadIntersection(intersection);
			ChangePhase(intersection.TrafficPhases.First().Name, TimeSpan.Zero);

			IBlackBox<double>? blackBox = intersection.GetNestModel();

			if (blackBox is null)
			{
				throw new ArgumentNullException(nameof(blackBox));
			}

			_blackBox = blackBox;
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			throw new NotImplementedException();
		}

		internal void ChangePhase(string nextTrafficPhaseName, TimeSpan timeElapsed)
		{
			if (_trafficPhasesHandler.CurrentPhase is null)
			{
				_trafficPhasesHandler.SetPhase(nextTrafficPhaseName, timeElapsed);
				CurrentPhaseTime = TimeSpan.Zero;
				return;
			}

			if (nextTrafficPhaseName != _trafficPhasesHandler.CurrentPhase.Name
				&& CurrentPhaseTime > MinimalTimeForOnePhase)
			{
				_trafficPhasesHandler.SetPhase(nextTrafficPhaseName, timeElapsed);
				CurrentPhaseTime = timeElapsed;
				return;
			}
			else
			{
				_trafficPhasesHandler.SetPhase(timeElapsed);
			}
		}
	}
}
