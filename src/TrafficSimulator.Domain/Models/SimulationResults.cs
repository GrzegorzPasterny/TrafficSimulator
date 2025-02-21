namespace TrafficSimulator.Domain.Models
{
	public class SimulationResults
	{
		public long TotalCalculationTimeMs { get; set; }
		public int CarsPassed { get; set; }
		public double TotalCarsIdleTimeMs { get; set; }
		public double AverageCarIdleTimeMs => TotalCarsIdleTimeMs / CarsPassed;

		public override string ToString()
		{
			return $"[TotalCalculationTimeMs = {TotalCalculationTimeMs}, " +
				$"CarsPassed = {CarsPassed}, " +
				$"AverageCarIdleTime = {AverageCarIdleTimeMs:F2}, " +
				$"TotalCarsIdleTimeMs = {TotalCarsIdleTimeMs:F1}]";
		}
	}
}