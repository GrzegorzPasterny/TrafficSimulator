using ErrorOr;

namespace TrafficSimulator.Application.Commons
{
	public static class ApplicationErrors
	{
		public static Error UnhandledSimulationException(Exception ex)
			=> Error.Unexpected("TrafficSimulator.Application.Simulation.Unexpected",
				$"Unexpected exception while running simulation [ErrorMessage = {ex.Message}]");

		internal static Error IntersectionUninitialized()
			=> Error.NotFound("TrafficSimulator.Application.IntersectionUninitialized",
				"Load the intersection simulation");
	}
}
