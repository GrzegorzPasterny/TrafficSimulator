namespace TrafficSimulator.Domain.Models
{
	public class SimulationResults
	{
		public long TotalCalculationTimeMs { get; set; }
		public int CarsPassed { get; set; }
		public int AverageCarIdleTime { get; set; }

		public override string ToString()
		{
			return $"[TotalCalculationTimeMs = {TotalCalculationTimeMs}, " +
				$"CarsPassed = {CarsPassed}, " +
				$"AverageCarIdleTime = {AverageCarIdleTime}]";
		}
	}
}