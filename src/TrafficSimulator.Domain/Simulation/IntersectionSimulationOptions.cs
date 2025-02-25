namespace TrafficSimulator.Domain.Models
{
	public class IntersectionSimulationOptions
	{
		public int MinimalDistanceBetweenTheCars { get; set; } = 5;
		public TimeSpan StepTimespan { get; set; } = TimeSpan.FromMilliseconds(100);
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
		public int StepLimit { get; set; } = 1000;
	}
}
