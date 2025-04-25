using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using SharpNeat;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Application.Commons;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Application.TrafficLights.Handlers.AI;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Agents;
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
		internal SimpleAiTrafficOutput? _simpleAiTrafficOutput;

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
			_simpleAiTrafficOutput = new SimpleAiTrafficOutput(_trafficPhasesHandler.TrafficPhases!);
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
			if (_simpleAiTrafficOutput == null)
			{
				return Task.FromResult(UnitResult.Failure(ApplicationErrors.IntersectionUninitialized()));
			}

			_blackBox.Reset();

			var inputs = _blackBox.Inputs.ToArray();
			var outputs = _blackBox.Outputs.Span;

			GetInputsForAiAgent(inputs).GetAwaiter().GetResult();

			// Run the model
			_blackBox.Activate();

			// Apply the results
			_simpleAiTrafficOutput.ApplyAiOutput(outputs);

			TrafficPhase bestTrafficPhase = _simpleAiTrafficOutput.BestTrafficPhase;

			CurrentPhaseTime += timeElapsed;
			ChangePhase(bestTrafficPhase.Name, timeElapsed);

			return Task.FromResult(UnitResult.Success<Error>());
		}

		private async Task GetInputsForAiAgent(double[] inputs)
		{
			const int numDirections = 4;
			if (inputs.Length < numDirections * 2)
			{
				throw new ArgumentException("Input span is too small.");
			}

			IEnumerable<Car> cars = await _sender.Send(new GetCarsCommand());

			// Group by direction
			var carsPerDirection = cars
				.Where(car => car.CurrentLocation.Location is InboundLane)
				.GroupBy(car => ((InboundLane)car.CurrentLocation.Location).WorldDirection)
				.ToDictionary(g => g.Key, g => g.ToList());

			// Make sure the ordering is consistent
			var orderedDirections = new[]
			{
				WorldDirection.North,
				WorldDirection.East,
				WorldDirection.South,
				WorldDirection.West
			};

			// Fill in car counts
			for (int i = 0; i < orderedDirections.Length; i++)
			{
				var dir = orderedDirections[i];
				inputs[i] = carsPerDirection.TryGetValue(dir, out var list) ? list.Count : 0;
			}

			// Fill in total waiting time
			for (int i = 0; i < orderedDirections.Length; i++)
			{
				var dir = orderedDirections[i];
				inputs[numDirections + i] = carsPerDirection.TryGetValue(dir, out var list)
					? list.Sum(c => c.MovesWhenCarWaited)
					: 0;
			}
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
			else
			{
				_trafficPhasesHandler.SetPhase(timeElapsed);
			}
		}
	}
}
