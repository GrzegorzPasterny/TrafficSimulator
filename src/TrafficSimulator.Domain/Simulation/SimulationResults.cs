namespace TrafficSimulator.Domain.Models
{
	public class SimulationResults
	{
		public long TotalCalculationTimeMs { get; set; }
		public int TotalCars { get; set; }
		public int CarsPassed { get; set; }
		public double TotalCarsIdleTimeMs { get; set; }
		public double AverageCarIdleTimeMs => TotalCarsIdleTimeMs / CarsPassed;
		public int SimulationStepsTaken { get; set; }

		public override string ToString()
		{
			return $"[TotalCalculationTimeMs = {TotalCalculationTimeMs}, " +
				$"CarsPassed = {CarsPassed}/{TotalCars}, " +
				$"SimulationStepsTaken = {SimulationStepsTaken}, " +
				$"AverageCarIdleTimeMs = {AverageCarIdleTimeMs:F2}, " +
				$"TotalCarsIdleTimeMs = {TotalCarsIdleTimeMs:F1}]";
		}
	}
}