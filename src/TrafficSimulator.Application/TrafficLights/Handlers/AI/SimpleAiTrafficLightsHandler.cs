using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.TrafficLights.Handlers.AI
{
	public class SimpleAiTrafficLightsHandler : ITrafficLightsHandler
	{
		internal readonly ISender _sender;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;
		private readonly IAiAgent _aiAgent;
		private readonly ILogger<SimpleAiTrafficLightsHandler> _logger;
		internal SimpleAiTrafficOutput? _simpleAiTrafficOutput;

		public TimeSpan CurrentPhaseTime { get; internal set; } = TimeSpan.Zero;
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
			_simpleAiTrafficOutput = new SimpleAiTrafficOutput(_trafficPhasesHandler.TrafficPhases!);
			ChangePhase(intersection.TrafficPhases.First().Name, TimeSpan.Zero);
		}

		public virtual async Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			if (_simpleAiTrafficOutput == null)
			{
				return UnitResult.Failure(ApplicationErrors.IntersectionUninitialized());
			}

			SimpleAiTrafficInput simpleAiTrafficInput = await GetInputsForAiAgent();

			// Run the model
			IReadOnlyList<float> output = _aiAgent.Predict(simpleAiTrafficInput.ToAiInput());

			// Apply the results
			_simpleAiTrafficOutput.ApplyAiOutput(output);

			TrafficPhase bestTrafficPhase = _simpleAiTrafficOutput.BestTrafficPhase;

			CurrentPhaseTime += timeElapsed;
			ChangePhase(bestTrafficPhase.Name, timeElapsed);

			return UnitResult.Success<Error>();
		}

		internal async Task<SimpleAiTrafficInput> GetInputsForAiAgent()
		{
			SimpleAiTrafficInput simpleAiTrafficInput = new SimpleAiTrafficInput();

			IEnumerable<Car> cars = await _sender.Send(new GetCarsCommand());
			IEnumerable<IGrouping<WorldDirection, Car>> carsPerDirection = cars.Where(car => car.CurrentLocation.Location is InboundLane)
				.GroupBy(car => (car.CurrentLocation.Location as InboundLane)!.WorldDirection)
				.OrderBy(carPerDirection => carPerDirection.Key);

			simpleAiTrafficInput.CarPerDirection =
				carsPerDirection.ToDictionary(c => c.Key, c => c.Count());
			simpleAiTrafficInput.TimeCarsSpentWaitingPerDirection =
				carsPerDirection.ToDictionary(c => c.Key,
											  c => c.Sum(car => car.MovesWhenCarWaited));
			return simpleAiTrafficInput;
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
		}
	}
}
