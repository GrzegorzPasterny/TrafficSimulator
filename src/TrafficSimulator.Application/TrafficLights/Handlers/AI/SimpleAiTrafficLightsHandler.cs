using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly ISender _sender;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly IAiAgent _aiAgent;
		private readonly ILogger<SimpleAiTrafficLightsHandler> _logger;

		public TimeSpan CurrentPhaseTime { get; private set; } = TimeSpan.Zero;
		// Options
		public TimeSpan MinimalTimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleAiTrafficLightsHandler(
			ISender sender, TrafficPhasesHandler trafficPhasesHandler, IAiAgent aiAgent, ILogger<SimpleAiTrafficLightsHandler> logger)
		{
			_sender = sender;
			_trafficPhasesHandler = trafficPhasesHandler;
			_aiAgent = aiAgent;
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
		}

		public Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			// Get the input for the model


			// Run the model

			// Apply the results

			throw new NotImplementedException();

		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			if (_trafficPhasesHandler.TrafficPhases is null)
			{
				// TODO: Handle
				throw new NotImplementedException();
			}

			ChangePhase(trafficPhaseName, TimeSpan.Zero);
			CurrentPhaseTime = TimeSpan.Zero;

			return UnitResult.Success<Error>();
		}

		private void ChangePhase(string desiredTrafficPhaseName, TimeSpan timeElapsed)
		{
			if (_trafficPhasesHandler.CurrentPhase is null)
			{
				_trafficPhasesHandler.SetPhase(desiredTrafficPhaseName, timeElapsed);
				CurrentPhaseTime = TimeSpan.Zero;
				return;
			}

			if (desiredTrafficPhaseName != _trafficPhasesHandler.CurrentPhase.Name && CurrentPhaseTime > MinimalTimeForOnePhase)
			{
				_trafficPhasesHandler.SetPhase(desiredTrafficPhaseName, timeElapsed);
				CurrentPhaseTime = timeElapsed;
				return;
			}
		}
	}
}
