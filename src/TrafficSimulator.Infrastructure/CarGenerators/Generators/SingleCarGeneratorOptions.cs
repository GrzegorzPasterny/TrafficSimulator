namespace TrafficSimulator.Infrastructure.CarGenerators.Generators
{
	public class SingleCarGeneratorOptions
	{
		// TODO: Generator needs to know somehow what simulation step is it in when generating the cars
		public TimeSpan DelayForGeneratingTheCar { get; set; } = TimeSpan.FromMilliseconds(200);
	}
}
