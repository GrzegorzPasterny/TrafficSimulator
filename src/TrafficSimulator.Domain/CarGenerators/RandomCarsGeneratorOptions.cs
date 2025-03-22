namespace TrafficSimulator.Domain.CarGenerators
{
	public class RandomCarsGeneratorOptions : CarGeneratorOptions
	{
		/// <summary>
		/// Expected average number of cars generated per second
		/// </summary>
		public double BaseRate { get; set; } = 1;

		public int AmountOfCarsToGenerate { get; set; } = 3;

	}
}
