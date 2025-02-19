using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Models.Agents
{
	public class Car : Entity
	{
		public readonly InboundLane StartLocation;
		public readonly List<LocationEntity> DistanceToCover;
		public LaneType CarTurnType { get; }

		public bool IsCarWaiting { get; private set; } = false;

		/// <summary>
		/// Velocity of the car in units per second
		/// </summary>
		public int Velocity { get; set; } = 50;

		/// <summary>
		/// Langth of the car
		/// </summary>
		public int Length { get; set; } = 2;

		public int MovesSoFar { get; private set; } = 0;
		public int MovesWhenCarWaited { get; private set; } = 0;

		public CarLocation CurrentLocation { get; private set; }

		public bool HasReachedDestination { get; private set; } = false;

		public Car(InboundLane startLocation)
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

			CarTurnType = StartLocation.TurnPossibilities.First().LaneType;

			WorldDirection outboundLaneWorldDirection = ((Lanes)StartLocation.Parent!).WorldDirection.Rotate(CarTurnType);

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

		public UnitResult<Error> Move(TimeSpan timeElapsed)
		{
			if (HasReachedDestination)
			{
				return DomainErrors.CarHasReachedDestination(Id);
			}

			MovesSoFar++;
			double distanceToGo = Velocity * timeElapsed.TotalSeconds;

			if (IsCarAtTrafficLights(distanceToGo, CurrentLocation))
			{
				if (CanCarPassTheTrafficLights() is false)
				{
					ApproachTheTrafficLightsAndStop();

					return UnitResult.Success<Error>();
				}
			}

			MoveTheCar(distanceToGo);

			DetermineIfCarReachedDestination();

			return UnitResult.Success<Error>();
		}

		private void MoveTheCar(double distanceToGo)
		{
			IsCarWaiting = false;

			if (distanceToGo > CurrentLocation.DistanceLeft)
			{
				int zeroBasedIndexOfCurrentLocation = DistanceToCover.IndexOf(CurrentLocation.Location);

				if (zeroBasedIndexOfCurrentLocation + 1 >= DistanceToCover.Count)
				{
					// Car reached the destination
					CurrentLocation.CurrentDistance = CurrentLocation.Location.Distance;
					return;
				}

				CurrentLocation.Location = DistanceToCover[zeroBasedIndexOfCurrentLocation + 1];
				CurrentLocation.CurrentDistance = distanceToGo - CurrentLocation.DistanceLeft;
			}
			else
			{
				CurrentLocation.CurrentDistance += distanceToGo;
				// Remove natural double rounding error.
				CurrentLocation.CurrentDistance = Math.Round(CurrentLocation.CurrentDistance, 5);
			}
		}

		private void DetermineIfCarReachedDestination()
		{
			if (CurrentLocation.Location == DistanceToCover.Last() && CurrentLocation.DistanceLeft == 0)
			{
				HasReachedDestination = true;
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
			TurnPossibility turnPossibility = StartLocation.TurnPossibilities.Single(t => t.LaneType == CarTurnType);

			return turnPossibility.TrafficLights!.TrafficLightState == Lights.TrafficLightState.Green;
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
				$"Velocity = {Velocity}, " +
				$"IsCarWaiting = {IsCarWaiting}]";
		}
	}
}
