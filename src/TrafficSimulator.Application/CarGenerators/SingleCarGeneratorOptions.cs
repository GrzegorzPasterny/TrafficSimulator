using TrafficSimulator.Domain.CarGenerators;

namespace TrafficSimulator.Application.Handlers.CarGenerators
{
	public class SingleCarGeneratorOptions : CarGeneratorOptions
	{
		// TODO: Generator needs to know somehow what simulation step is it in when generating the cars
		public TimeSpan DelayForGeneratingTheCar { get; set; } = TimeSpan.FromMilliseconds(200);
	}
}
