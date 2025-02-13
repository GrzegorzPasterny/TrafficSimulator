using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class Car : Entity
	{
		private readonly Lane _startLocation;
		private readonly Lane _endLocation;
		private readonly IntersectionCore _intersectionCore;
		public readonly List<LocationEntity> DistanceToCover;

		public Car(Lane startLocation, Lane endLocation)
		{
			_startLocation = startLocation;
			_endLocation = endLocation;
			_intersectionCore = startLocation.IntersectionCore;
			CurrentLocation = new(startLocation, 0);
			DistanceToCover = [_startLocation, _intersectionCore, _endLocation];
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
