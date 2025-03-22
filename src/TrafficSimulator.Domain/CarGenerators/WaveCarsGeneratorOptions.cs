namespace TrafficSimulator.Domain.CarGenerators
{
	public class WaveCarsGeneratorOptions : CarGeneratorOptions
	{
		public int AmountOfCarsToGenerate { get; set; } = 20;

		/// <summary>
		/// Expected average number of cars generated per second
		/// </summary>
		public double BaseRate { get; set; } = 1;

		/// <summary>
		/// Cars generation probability wave cycles per seconds
		/// </summary>
		public double WavePeriodHz { get; set; } = 5;
		public double WaveAmplitude { get; set; } = 30;
	}
}
