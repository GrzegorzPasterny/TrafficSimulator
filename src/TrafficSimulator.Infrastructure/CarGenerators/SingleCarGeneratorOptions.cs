namespace TrafficSimulator.Infrastructure.CarGenerators
{
	public class SingleCarGeneratorOptions
	{
		public TimeSpan DelayForGeneratingTheCar { get; set; } = TimeSpan.FromSeconds(2);
	}
}
