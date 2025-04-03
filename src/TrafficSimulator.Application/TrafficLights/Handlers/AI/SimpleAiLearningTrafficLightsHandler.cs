using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.AI;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiLearningTrafficLightsHandler : SimpleAiTrafficLightsHandler
	{
		private readonly IAiLearningAgent _aiAgent;

		public SimpleAiLearningTrafficLightsHandler(
			ISender sender, TrafficPhasesHandler trafficPhasesHandler,
			IAiLearningAgent aiAgent, ILogger<SimpleAiTrafficLightsHandler> logger)
			: base(sender, trafficPhasesHandler, aiAgent, logger)
		{
			_aiAgent = aiAgent;
		}

		public override async Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			if (_simpleAiTrafficOutput == null)
			{
				return UnitResult.Failure(ApplicationErrors.IntersectionUninitialized());
			}

			SimpleAiTrafficInput simpleAiTrafficInput = await GetInputsForAiAgent();

			// Get AI decision
			IReadOnlyList<float> qValues = _aiAgent.Predict(simpleAiTrafficInput.ToAiInput());
			_simpleAiTrafficOutput.ApplyAiOutput(qValues);
			TrafficPhase bestTrafficPhase = _simpleAiTrafficOutput.BestTrafficPhase;

			// Update RL Training Data
			var trafficState = new TrafficState
			{
				Inputs = simpleAiTrafficInput.ToAiInput().ToArray(),
				QValues = qValues.ToArray()
			};
			_aiAgent.CollectTrainingData(trafficState);

			CurrentPhaseTime += timeElapsed;
			ChangePhase(bestTrafficPhase.Name, timeElapsed);

			TrainAiModel();

			return UnitResult.Success<Error>();
		}

		private void TrainAiModel()
		{
			_aiAgent.TrainModel();
		}
	}
}
