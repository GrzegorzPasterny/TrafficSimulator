using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Handlers.TrafficPhases;
using TrafficSimulator.Domain.Models.Agents;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Application.Handlers.Lights
{
	public class SimpleDynamicTrafficLightsHandler : ITrafficLightsHandler
	{
		private readonly ICarRepository _carRepository;
		private readonly TrafficPhasesHandler _trafficPhasesHandler;

		public TimeSpan CurrentPhaseTime { get; set; }

		// Options
		public TimeSpan MinimalTimeForOnePhase { get; set; } = TimeSpan.FromSeconds(1);

		public SimpleDynamicTrafficLightsHandler(ICarRepository carRepository, TrafficPhasesHandler trafficPhasesHandler)
		{
			_carRepository = carRepository;
			_trafficPhasesHandler = trafficPhasesHandler;
		}

		public async Task<UnitResult<Error>> SetLights(TimeSpan timeElapsed)
		{
			IEnumerable<Car> waitingCars = (await _carRepository.GetCarsAsync()).Where(car => car.IsCarWaiting);

			IOrderedEnumerable<IGrouping<InboundLane, Car>> carsWaitingOnInboundLanes = waitingCars.GroupBy(car => car.StartLocation).OrderDescending();

			InboundLane mostCrowdedInboundLane = carsWaitingOnInboundLanes.First().Key;

			IEnumerable<TrafficPhase> desiredTrafficPhases =
				_trafficPhasesHandler.TrafficPhases.Where(p => p.LanesWithGreenLight.Any(l => l.InboundLane == mostCrowdedInboundLane));

			// TODO: The first one doesn't mean the best one
			// TODO: Calculations above take into account only Inbound lane,
			// but there is possibility that some of the cars want to turn other direction from the same lane
			// that the traffic light does not allow to (e.g. conditional green for right turn)

			// TODO: Finish implementation

			return UnitResult.Success<Error>();
		}

		public void LoadIntersection(Intersection intersection)
		{
			_trafficPhasesHandler.LoadIntersection(intersection);
		}

		public TrafficPhase GetCurrentTrafficPhase()
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> SetLightsManually(string trafficPhaseName)
		{
			throw new NotImplementedException();
		}
	}
}
