using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Models.Agents
{
	public class Car : Entity<Guid>
	{
		public readonly InboundLane StartLocation;
		public readonly List<LocationEntity> DistanceToCover;
		public LaneType CarTurnType { get; }

		public bool IsCarWaiting { get; private set; } = false;

		/// <summary>
		/// Velocity of the car in units per second when it is moving
		/// </summary>
		public int MoveVelocity { get; set; } = 50;

		/// <summary>
		/// Length of the car
		/// </summary>
		public int Length { get; set; } = 2;

		public int MovesSoFar { get; private set; } = 0;
		public int MovesWhenCarWaited { get; private set; } = 0;

		public CarLocation CurrentLocation { get; private set; }

		public bool HasReachedDestination { get; private set; } = false;

		/// <summary>
		/// Informs about reaching the destination in a previous step
		/// </summary>
		public bool HasJustReachedDestination { get; private set; } = false;

		public LocationEntity? NextLocation
		{
			get
			{
				if (CurrentLocation.Location == DistanceToCover.Last())
				{
					return null;
				}
				else
				{
					int currentLocationIndex = DistanceToCover.FindIndex(l => l == CurrentLocation.Location);

					return DistanceToCover[currentLocationIndex + 1];
				}
			}
		}

		public Car(InboundLane startLocation) : base(Guid.NewGuid())
		{
			StartLocation = startLocation;
			CurrentLocation = new(startLocation, 0);

			// TODO: Randomize the logic where car can go
			Intersection intersection = StartLocation.Root;
			IntersectionCore? intersectionCore = intersection.IntersectionCore;

			if (intersectionCore is null)
			{
				// TODO: Handle
			}

			// TODO: Randomize
			CarTurnType = StartLocation.LaneTypes.First();

			WorldDirection outboundLaneWorldDirection = StartLocation.WorldDirection.Rotate(CarTurnType);

			Lanes? lanes = intersection.LanesCollection.Find(lanes => lanes.WorldDirection == outboundLaneWorldDirection);

			if (lanes is null || lanes.OutboundLanes?.Count == 0)
			{
				// TODO: Handle
			}

			OutboundLane? carEndLocation = lanes?.OutboundLanes?.First();

			if (carEndLocation is null)
			{
				// TODO: Handle
			}

			DistanceToCover = [StartLocation, intersectionCore!, carEndLocation!];
		}

		public UnitResult<Error> Move(TimeSpan timeElapsed, IEnumerable<Car> cars, int minimalDistanceBetweenCars = 1)
		{
			if (HasJustReachedDestination)
			{
				HasJustReachedDestination = false;
			}

			if (HasReachedDestination)
			{
				return DomainErrors.CarHasReachedDestination(Id);
			}

			MovesSoFar++;
			double distanceToGo = CalculateDistance(timeElapsed, cars, minimalDistanceBetweenCars, out bool isCarMovingToNextLocation);

			if (IsCarAtTrafficLights(distanceToGo, CurrentLocation))
			{
				if (CanCarPassTheTrafficLights() is false)
				{
					ApproachTheTrafficLightsAndStop();

					return UnitResult.Success<Error>();
				}
			}

			MoveTheCar(distanceToGo, isCarMovingToNextLocation);

			DetermineIfCarReachedDestination();

			return UnitResult.Success<Error>();
		}

		/// <summary>
		/// Calculates the distance that car is allowed to for next <paramref name="timeElapsed"/>.
		/// It takes into account other <paramref name="cars"/> and required <paramref name="minimalDistanceBetweenCars"/>
		/// </summary>
		/// <param name="timeElapsed"></param>
		/// <param name="cars"></param>
		/// <param name="minimalDistanceBetweenCars"></param>
		/// <returns></returns>
		private double CalculateDistance(TimeSpan timeElapsed, IEnumerable<Car> cars, int minimalDistanceBetweenCars, out bool moveToNextLocation)
		{
			double distanceToGo = MoveVelocity * timeElapsed.TotalSeconds;
			moveToNextLocation = false;

			if (IsCarMovingToNextLocation(distanceToGo))
			{
				Car? otherCarOnTheSameOrNextLane = FindCarInFrontOnTheSameOrNextLane(cars);

				if (otherCarOnTheSameOrNextLane is not null)
				{
					double distanceBetweenTheCars = MeasureCarsDistance(otherCarOnTheSameOrNextLane, this, minimalDistanceBetweenCars);

					if (distanceBetweenTheCars <= 0)
					{
						return 0;
					}

					distanceToGo = Math.Min(distanceToGo, distanceBetweenTheCars);
				}

				if (IsCarMovingToNextLocation(distanceToGo) == false)
				{
					return distanceToGo;
				}

				moveToNextLocation = true;
			}
			else
			{
				Car? otherCarOnTheSameLane = FindCarInFrontOnTheSameLane(cars);

				if (otherCarOnTheSameLane is not null)
				{
					double distanceBetweenTheCars = MeasureCarsDistance(otherCarOnTheSameLane, this, minimalDistanceBetweenCars);

					if (distanceBetweenTheCars <= 0)
					{
						return 0;
					}

					distanceToGo = Math.Min(distanceToGo, distanceBetweenTheCars);
				}
			}

			return distanceToGo;
		}

		private void MoveTheCar(double distanceToGo, bool isCarMovingToNextLocation)
		{
			if (distanceToGo <= 0)
			{
				IsCarWaiting = true;
				MovesWhenCarWaited++;
				return;
			}

			IsCarWaiting = false;

			if (isCarMovingToNextLocation)
			{
				int zeroBasedIndexOfCurrentLocation = DistanceToCover.IndexOf(CurrentLocation.Location);

				if (zeroBasedIndexOfCurrentLocation + 1 >= DistanceToCover.Count)
				{
					// Car reached the destination
					CurrentLocation.CurrentDistance = CurrentLocation.Location.Distance;
					return;
				}

				CurrentLocation.Location = DistanceToCover[zeroBasedIndexOfCurrentLocation + 1];
				CurrentLocation.CurrentDistance = Math.Round(distanceToGo - CurrentLocation.DistanceLeft, 5);
			}
			else
			{
				MoveCarWithMathRounding(distanceToGo);
			}
		}

		private void MoveCarWithMathRounding(double distanceToGo)
		{
			CurrentLocation.CurrentDistance += distanceToGo;
			// Remove natural double rounding error.
			CurrentLocation.CurrentDistance = Math.Round(CurrentLocation.CurrentDistance, 5);
		}

		private bool IsCarMovingToNextLocation(double distanceToGo)
		{
			return distanceToGo > CurrentLocation.DistanceLeft;
		}

		/// <summary>
		/// Measures <paramref name="car1"/> and <paramref name="car2"/> distance 
		/// assuming that <paramref name="car1"/> is further than <paramref name="car2"/> 
		/// and the locations of both cars are same, or adjacent.
		/// It is taking into account the length and <paramref name="minimalDistanceBetweenCars"/>
		/// </summary>
		/// <param name="car1">Car in front</param>
		/// <param name="car2">Car in back</param>
		/// <returns></returns>
		private double MeasureCarsDistance(Car car1, Car car2, int minimalDistanceBetweenCars)
		{
			if (car1.CurrentLocation.Location == car2.CurrentLocation.Location)
			{
				return car1.CurrentLocation.CurrentDistance
					- car2.CurrentLocation.CurrentDistance
					- minimalDistanceBetweenCars
					- car1.Length / 2
					- car2.Length / 2;
			}

			// Assuming that cars are in adjacent location
			return
				car1.CurrentLocation.CurrentDistance
				+ car2.CurrentLocation.DistanceLeft
				- minimalDistanceBetweenCars
				- car1.Length / 2
				- car2.Length / 2;
		}

		private Car? FindCarInFrontOnTheSameLane(IEnumerable<Car> cars)
		{
			return cars.Where(car => car.HasReachedDestination == false)
				.Where(car => car.CurrentLocation.Location == CurrentLocation.Location && car.Id != Id)
				.Where(car => car.CurrentLocation.DistanceLeft < CurrentLocation.DistanceLeft)
				.OrderByDescending(c => c.CurrentLocation.DistanceLeft)
				.FirstOrDefault();
		}

		private Car? FindCarInFrontOnTheSameOrNextLane(IEnumerable<Car> cars)
		{
			Car? car = FindCarInFrontOnTheSameLane(cars);

			if (car is null)
			{
				return cars.Where(car => car.HasReachedDestination == false)
					.Where(car =>
						car.Id != Id
						&& car.NextLocation is not null
						&& NextLocation is not null
						&& car.CurrentLocation.Location == NextLocation
					)
					.OrderBy(car => car.NextLocation!.Distance)
					.FirstOrDefault();
			}

			return car;
		}

		private void DetermineIfCarReachedDestination()
		{
			if (CurrentLocation.Location == DistanceToCover.Last() && CurrentLocation.DistanceLeft == 0)
			{
				HasJustReachedDestination = true;
				HasReachedDestination = true;
				IsCarWaiting = false;
			}
		}

		private void ApproachTheTrafficLightsAndStop()
		{
			// Move as far as possible to the Traffic Lights
			CurrentLocation.CurrentDistance = CurrentLocation.Location.Distance;

			IsCarWaiting = true;
			MovesWhenCarWaited++;
		}

		private bool CanCarPassTheTrafficLights()
		{
			return StartLocation.TrafficLights!.TrafficLightState == TrafficLightState.Green;
		}

		private bool IsCarAtTrafficLights(double distanceToGo, CarLocation currentLocation)
		{
			return currentLocation.Location is InboundLane && currentLocation.DistanceLeft < distanceToGo;
		}

		public override string ToString()
		{
			return
				$"[CarId = {Id}, " +
				$"HasReachedDestination = {HasReachedDestination}, " +
				$"Location = {CurrentLocation.Location.Name}, " +
				$"Distance = {CurrentLocation.CurrentDistance}, " +
				$"Velocity = {MoveVelocity}, " +
				$"IsCarWaiting = {IsCarWaiting}]";
		}
	}
}
