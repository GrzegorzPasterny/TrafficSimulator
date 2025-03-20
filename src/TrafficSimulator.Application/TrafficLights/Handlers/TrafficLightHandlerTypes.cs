namespace TrafficSimulator.Application.TrafficLights.Handlers
{
	public static class TrafficLightHandlerTypes
	{
		public const string Default = Sequential;

		public const string Sequential = "Sequential";
		public const string Manual = "Manual";

		public static IReadOnlyList<string> Modes = [Sequential, Manual];
	}
}
