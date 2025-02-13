using CSharpFunctionalExtensions;

namespace TrafficSimulator.Domain.Models
{
	public class Car : Entity
	{
		/// <summary>
		/// Velocity of the car in units per second
		/// </summary>
		public int Velocity { get; set; } = 5;

		/// <summary>
		/// Langth of the car
		/// </summary>
		public int Length { get; set; }

		public Location Location { get; private set; }
	}
}
