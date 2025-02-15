using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Models.Agents
{
	public class Car : Entity
	{
		private readonly OutboundLane _startLocation;
		public readonly List<LocationEntity> DistanceToCover;

		public Car(OutboundLane startLocation)
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

			LaneType carTurnType = _startLocation.LaneType.First();

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
		public int Velocity { get; set; } = 5;

		/// <summary>
		/// Langth of the car
		/// </summary>
		public int Length { get; set; } = 2;

		public CarLocation CurrentLocation { get; private set; }

		public bool HasReachedDestination { get; private set; } = false;

		public UnitResult<Error> Move(int timeElapsed)
		{
			if (HasReachedDestination)
			{
				return DomainErrors.CarHasReachedDestination(Id);
			}

			int distanceToGo = Velocity * timeElapsed;

			if (distanceToGo > CurrentLocation.DistanceLeft)
			{
				int zeroBasedIndexOfCurrentLocation = DistanceToCover.IndexOf(CurrentLocation.Location);

				if (zeroBasedIndexOfCurrentLocation++ >= DistanceToCover.Count)
				{
					HasReachedDestination = true;
					return UnitResult.Success<Error>();
				}

				CurrentLocation.Location = DistanceToCover[zeroBasedIndexOfCurrentLocation++];
				CurrentLocation.CurrentDistance = distanceToGo - CurrentLocation.CurrentDistance;
			}
			else
			{
				CurrentLocation.CurrentDistance += distanceToGo;
			}

			return UnitResult.Success<Error>();
		}
	}
}
