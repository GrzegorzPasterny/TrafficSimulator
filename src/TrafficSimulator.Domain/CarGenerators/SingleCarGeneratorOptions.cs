using TrafficSimulator.Domain.CarGenerators;

namespace TrafficSimulator.Domain.Handlers.CarGenerators
{
	public class SingleCarGeneratorOptions : CarGeneratorOptions
	{
		public TimeSpan DelayForGeneratingTheCar { get; set; } = TimeSpan.FromMilliseconds(200);
	}
}
