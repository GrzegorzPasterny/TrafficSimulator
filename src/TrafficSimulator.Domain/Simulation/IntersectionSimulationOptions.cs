namespace TrafficSimulator.Domain.Models
{
	public class IntersectionSimulationOptions
	{
		public int MinimalDistanceBetweenTheCars { get; set; } = 1;
		public TimeSpan StepTimespan { get; set; } = TimeSpan.FromMilliseconds(40);
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
		public int StepLimit { get; set; } = 1000;
	}
}
