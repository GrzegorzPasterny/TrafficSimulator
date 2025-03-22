namespace TrafficSimulator.Domain.CarGenerators
{
	public class WaveCarsGeneratorOptions : CarGeneratorOptions
	{
		public int TotalCarsToGenerate { get; set; } = 20;
		public double BaseProbability { get; set; } = 50;

		/// <summary>
		/// Cars generation probability wave cycles per seconds
		/// </summary>
		public double WavePeriodHz { get; set; } = 5;
		public double WaveAmplitude { get; set; } = 30;
	}
}
