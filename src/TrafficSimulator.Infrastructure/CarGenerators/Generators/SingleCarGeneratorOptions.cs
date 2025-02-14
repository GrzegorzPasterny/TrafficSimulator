namespace TrafficSimulator.Infrastructure.CarGenerators.Generators
{
    public class SingleCarGeneratorOptions
    {
        public TimeSpan DelayForGeneratingTheCar { get; set; } = TimeSpan.FromSeconds(2);
    }
}
