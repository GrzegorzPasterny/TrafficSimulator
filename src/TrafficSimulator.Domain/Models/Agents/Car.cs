using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Models.Agents
{
	public class Car : Entity
	{
		private readonly InboundLane _startLocation;
		public readonly List<LocationEntity> DistanceToCover;

		public Car(InboundLane startLocation)
		{
			_startLocation = startLocation;
			CurrentLocation = new(startLocation, 0);

			// TODO: Randomize the logic where can can go
			Intersection intersection = _startLocation.Root;
			IntersectionCore? intersectionCore = intersection.IntersectionCore;

			if (intersectionCore is null)
			{
				// TODO: Handle
			}

			LaneType carTurnType = _startLocation.LaneTypes.First();

			WorldDirection outboundLaneWorldDirection = ((Lanes)_startLocation.Parent!).WorldDirection.Rotate(carTurnType);

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

			DistanceToCover = [_startLocation, intersectionCore!, carEndLocation!];
		}

		/// <summary>
		/// Velocity of the car in units per second
		/// </summary>
		public int Velocity { get; set; } = 50;

		/// <summary>
		/// Langth of the car
		/// </summary>
		public int Length { get; set; } = 2;

		public int MovesSoFar { get; private set; } = 0;

		public CarLocation CurrentLocation { get; private set; }

		public bool HasReachedDestination { get; private set; } = false;

		public UnitResult<Error> Move(TimeSpan timeElapsed)
		{
			if (HasReachedDestination)
			{
				return DomainErrors.CarHasReachedDestination(Id);
			}

			MovesSoFar++;
			double distanceToGo = Velocity * timeElapsed.TotalSeconds;

			if (distanceToGo > CurrentLocation.DistanceLeft)
			{
				int zeroBasedIndexOfCurrentLocation = DistanceToCover.IndexOf(CurrentLocation.Location);

				if (zeroBasedIndexOfCurrentLocation + 1 >= DistanceToCover.Count)
				{
					CurrentLocation.CurrentDistance = CurrentLocation.Location.Distance;
					HasReachedDestination = true;
					return UnitResult.Success<Error>();
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

			if (CurrentLocation.Location == DistanceToCover.Last() && CurrentLocation.DistanceLeft == 0)
			{
				HasReachedDestination = true;
			}

			return UnitResult.Success<Error>();
		}

		public override string ToString()
		{
			return
				$"[CarId = {Id}, " +
				$"HasReachedDestination = {HasReachedDestination}, " +
				$"Location = {CurrentLocation.Location.Name}, " +
				$"Distance = {CurrentLocation.CurrentDistance}, " +
				$"Velocity = {Velocity}]";
		}
	}
}
