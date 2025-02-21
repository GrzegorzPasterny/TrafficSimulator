namespace TrafficSimulator.Application.Handlers.CarGenerators
{
	public class MultipleCarsGeneratorOptions
	{
		public int AmountOfCarsToGenerate { get; set; } = 3;
		public TimeSpan DelayBetweenCarGeneration { get; set; } = TimeSpan.FromMilliseconds(500);
	}
}
