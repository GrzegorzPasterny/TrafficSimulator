using CSharpFunctionalExtensions;
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

		internal static Error TrafficLightsChangeAttemptedTooSoon(string currentPhaseName, int currentPhaseDurationMs, string nextPhaseName)
			=> Error.Conflict("TrafficSimulator.Application.TrafficLightsChangeAttemptedTooSoon",
				$"Attempt to change the lights too fast " +
				$"[CurrentPhaseName = {currentPhaseName}, CurrentPhaseDurationMs = {currentPhaseDurationMs}," +
				$"NextPhaseName = {nextPhaseName}]");

		internal static UnitResult<Error> TrafficPhaseManualChangeAttemptInInMemoryMode()
			=> Error.Forbidden("TrafficSimulator.Application.InMemoryModeInvalidAction.TrafficPhaseManualChangeAttempt",
				"In Memory simulation mode does not support manual changes of the Traffic Phase");
	}
}
