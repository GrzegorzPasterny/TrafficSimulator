using FluentAssertions;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.IntersectionSimulations;
using TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Infrastructure.UnitTests.SimulationSetup.Json
{
	public class JsonSimulationSetupRepositoryTests
	{
		[Fact]
		public void SaveIntersectionSimulationToJson_ShouldProduceFileOnTheDisc()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation = new(IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest);

			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new JsonSimulationSetupRepository(new IntersectionSimulationDtoMapper());

			// Act
			jsonSimulationSetupRepository.Save(intersectionSimulation).IsSuccess.Should().BeTrue();

			// Assert
			File.Exists(Path.Combine(
					jsonSimulationSetupRepository.Options.DirectoryPath,
					$"{intersectionSimulation.Name}_{intersectionSimulation.Id}.json")
				).Should().BeTrue();
		}

		[Fact]
		public void LoadIntersectionSimulation_ShouldReturnCorrectSimulation_WhenFileExists()
		{
			// Arrange
			var intersectionSimulation = new IntersectionSimulation(
				IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest);
			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new JsonSimulationSetupRepository(new IntersectionSimulationDtoMapper());

			// Save a test simulation to the file system
			jsonSimulationSetupRepository.Save(intersectionSimulation);

			// Act
			var result = jsonSimulationSetupRepository.Load(intersectionSimulation.Id);

			// Assert
			result.IsError.Should().BeFalse();
			result.Value.Should().BeEquivalentTo(intersectionSimulation);
		}

	}
}
