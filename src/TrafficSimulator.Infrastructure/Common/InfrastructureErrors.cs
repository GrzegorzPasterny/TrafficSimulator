using ErrorOr;

namespace TrafficSimulator.Infrastructure.Common
{
	public static class InfrastructureErrors
	{
		internal static Error IntersectionNotSet()
			=> Error.NotFound("TrafficSimulator.Intersection.NotSet", "Intersection has not been set");

		internal static Error CarGeneratorAlreadyStarted()
		{
			throw new NotImplementedException();
		}

		internal static Error JsonSaveFailure(string errorMessage)
			=> Error.Failure("TrafficSimulator.Infrastructure.JsonSaveFailed", $"Failed to save simulation object: {errorMessage}");

		internal static Error FileNotFound(string identifier)
			=> Error.Failure("TrafficSimulator.Infrastructure.FileNotFound", $"Failed to find file on the disc [FileName = {identifier}]");

		internal static Error FileExtensionIncorrect(string identifier, string extension)
			=> Error.Failure("TrafficSimulator.Infrastructure.JsonSaveFailed", $"Invalid file extension [FileName = {identifier}, DemandedExtension = {extension}]");

	}
}
