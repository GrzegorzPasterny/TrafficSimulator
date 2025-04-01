namespace TrafficSimulator.Application.TrafficLights.Handlers.Factory
{
	public static class TrafficLightHandlerTypes
	{
		public const string Default = Sequential;

		public const string Sequential = "Sequential";
		public const string Manual = "Manual";
		public const string Dynamic = "Dynamic";

		public static IReadOnlyList<string> Modes = [Sequential, Manual, Dynamic];
	}
}
