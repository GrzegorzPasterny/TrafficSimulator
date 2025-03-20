namespace TrafficSimulator.Domain.Cars
{
	public class CarOptions
	{
		/// <summary>
		/// Velocity of the car in units per second when it is moving
		/// </summary>
		public int MoveVelocity { get; set; } = 50;

		/// <summary>
		/// Length of the car
		/// </summary>
		public int Length { get; set; } = 2;

		public int DistanceBetweenCars { get; set; } = 1;
	}
}
