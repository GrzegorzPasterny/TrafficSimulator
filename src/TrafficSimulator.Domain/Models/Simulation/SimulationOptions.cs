namespace TrafficSimulator.Domain.Models
{
	public class SimulationOptions
	{
		public int MinimalDistanceBetweenTheCars { get; set; } = 5;
		public TimeSpan Step { get; set; } = TimeSpan.FromMilliseconds(100);
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
	}
}
