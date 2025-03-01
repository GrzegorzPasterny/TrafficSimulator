using TrafficSimulator.Domain.CarGenerators;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public class MultipleCarsGeneratorOptions : CarGeneratorOptions
	{
		public int AmountOfCarsToGenerate { get; set; } = 3;
		public TimeSpan DelayBetweenCarGeneration { get; set; } = TimeSpan.FromMilliseconds(500);
	}
}
