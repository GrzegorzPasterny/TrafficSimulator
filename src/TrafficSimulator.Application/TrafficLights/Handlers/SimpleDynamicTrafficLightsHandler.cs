using CSharpFunctionalExtensions;
using ErrorOr;
using MediatR;
using TrafficSimulator.Application.Cars.GetCars;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	/// <summary>
	/// Traffic Lights handler that always sets green for inbound lane with the most waiting cars
	/// </summary>
	public class SimpleDynamicTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly ISender _sender;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;

		public TimeSpan CurrentPhaseTime { get; private set; } = TimeSpan.Zero;

		// Options
		public TimeSpan MinimalTimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleDynamicTrafficLightsHandler(ISender sender, TrafficPhasesHandler trafficPhasesHandler)
		{
			_sender = sender;
			_trafficPhasesHandler = trafficPhasesHandler;
		}

		public async Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			CurrentPhaseTime += timeElapsed;

			if (CurrentPhaseTime < MinimalTimeForOnePhase)
			{
				_trafficPhasesHandler.SetPhase(timeElapsed);
				return UnitResult.Success<Error>();
			}

			IEnumerable<Car> cars = await _sender.Send(new GetCarsCommand());
			IEnumerable<Car> waitingCars = cars.Where(car => car.IsCarWaiting);

			if (waitingCars is null || waitingCars.Count() == 0)
			{
				// Simulation has not started, or is in the starting phase
				_trafficPhasesHandler.SetPhase(timeElapsed);
				return UnitResult.Success<Error>();
			}

			var waitingCarsOnLanes = waitingCars
				.GroupBy(car => car.StartLocation);

			InboundLane mostCrowdedInboundLane = waitingCarsOnLanes
				.MaxBy(group => group.Count())?.Key;

			IEnumerable<TrafficPhase> desiredTrafficPhases =
				_trafficPhasesHandler.TrafficPhases!
					.Where(p => p.LanesWithGreenLight.Any(l => l.InboundLane == mostCrowdedInboundLane));

			// Pick traffic phase randomly, when more than one phase switch green light for the most crowded inbound
			// TODO: Take into account conditional green light for right turn
			TrafficPhase desiredTrafficPhase =
				desiredTrafficPhases.ElementAt(Random.Shared.Next(0, desiredTrafficPhases.Count()));

			ChangePhase(desiredTrafficPhase.Name, timeElapsed);

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

		public void LoadIntersection(Intersection intersection)
		{
			_trafficPhasesHandler.LoadIntersection(intersection);
			ChangePhase(intersection.TrafficPhases.First().Name, TimeSpan.Zero);
		}

		public TrafficPhase? GetCurrentTrafficPhase()
		{
			return _trafficPhasesHandler.CurrentPhase;
		}

		/// <summary>
		/// Changes the Traffic phase according to <paramref name="trafficPhaseName"/>
		/// Not recomended in dynamic mode.
		/// </summary>
		/// <param name="trafficPhaseName"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException">When Intersection has not yet been loaded</exception>
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
	}
}
