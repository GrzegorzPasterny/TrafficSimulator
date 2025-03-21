namespace TrafficSimulator.Domain.CarGenerators
{
	public class RandomCarsGeneratorOptions : CarGeneratorOptions
	{
		/// <summary>
		/// Probability in percents of car generation in current simulation step.
		/// Range from 1 to 100
		/// </summary>
		public int Probability { get; set; } = 20;

		public int AmountOfCarsToGenerate { get; set; } = 3;

	}
}
