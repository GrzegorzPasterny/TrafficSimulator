namespace TrafficSimulator.Domain.Simulation
{
	public static class SimulationMode
	{
		public const string RealTime = "RealTime";
		public const string InMemory = "InMemory";

		public static IReadOnlyList<string> Modes = [RealTime, InMemory];
	}
}
