namespace TrafficSimulator.Domain.Models
{
	public class IntersectionSimulationOptions
	{
		public TimeSpan StepTimespan { get; set; } = TimeSpan.FromMilliseconds(40);
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
		public int StepLimit { get; set; } = 2000;
		public string? TrafficLightHandlerType { get; set; } = "Sequential";
		public bool SaveSimulationSnapshots { get; set; } = false;

		public override string ToString()
		{
			return $"[StepTimespan = {StepTimespan}, Timeout = {Timeout}, " +
				$"StepLimit = {StepLimit}, TrafficLightsHandlerType = {TrafficLightHandlerType}]";
		}
	}
}
