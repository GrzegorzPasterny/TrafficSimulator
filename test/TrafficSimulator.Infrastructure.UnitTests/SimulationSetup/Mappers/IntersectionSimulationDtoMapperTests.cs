using ErrorOr;
using FluentAssertions;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DTOs;
using TrafficSimulator.Infrastructure.IntersectionSimulations;
using TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence;
using TrafficSimulator.Infrastructure.UnitTests.Assets;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Infrastructure.UnitTests.SimulationSetup.Mappers
{
	public class IntersectionSimulationDtoMapperTests
	{
		[Fact]
		public void MapFromDomainToDto_GivenZebraCrossing_ShouldProduceDtosAsExpected()
		{
			// Arrange
			IntersectionSimulationDtoMapper mapper = new IntersectionSimulationDtoMapper();

			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;

			// Act
			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto = mapper.ToDto(intersectionSimulation);

			// Assert
			intersectionSimulationDto.IsError.Should().BeFalse();
			intersectionSimulationDto.Value.Should().BeEquivalentTo(IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest,
				options => options.Excluding(x => x.Id));
		}

		[Fact]
		public void MapFromDtoToDomain_GivenZebraCrossingDto_ShouldProduceDomainObjectsAsExpected()
		{
			// Arrange
			IntersectionSimulationDtoMapper mapper = new IntersectionSimulationDtoMapper();

			IntersectionSimulationDto intersectionSimulationDto = IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest;

			// Act
			ErrorOr<IntersectionSimulation> intersectionSimulation = mapper.ToDomain(intersectionSimulationDto);

			// Assert
			intersectionSimulation.IsError.Should().BeFalse();

			// This assertion does not entirely work
			intersectionSimulation.Value.Intersection.Should().BeEquivalentTo(IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest,
				options => options
					//.ComparingByMembers<IntersectionObject>()
					//.ComparingByMembers<IntersectionCore>()
					//.Excluding(x => x.Parent)
					//.Excluding(x => x.Root)
					);
		}

		[Fact]
		public void MapFromDtoToDomainAndBackToDto_GivenZebraCrossingDto_DtosObjectsShouldBeTheSame()
		{
			// Arrange
			IntersectionSimulationDtoMapper mapper = new IntersectionSimulationDtoMapper();

			IntersectionSimulationDto intersectionSimulationDto = IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest;

			// Act
			ErrorOr<IntersectionSimulation> intersectionSimulation = mapper.ToDomain(intersectionSimulationDto);

			intersectionSimulation.IsError.Should().BeFalse();

			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto2Result = mapper.ToDto(intersectionSimulation.Value);

			// Assert
			intersectionSimulationDto2Result.IsError.Should().BeFalse();
			intersectionSimulationDto2Result.Value.Should().BeEquivalentTo(intersectionSimulationDto);
		}

		[Fact]
		public void LoadAllIntersectionSimulations_ShouldReturnListOfSimulations_WhenFilesExist()
		{
			// Arrange
			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new JsonSimulationSetupRepository(new IntersectionSimulationDtoMapper());

			// Ensure the directory is empty by deleting any existing files
			var directoryPath = jsonSimulationSetupRepository.Options.DirectoryPath;
			if (Directory.Exists(directoryPath))
			{
				var existingFiles = Directory.GetFiles(directoryPath);
				foreach (var file in existingFiles)
				{
					File.Delete(file);
				}
			}

			// Create and save a few test simulations
			var intersectionSimulation1 = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			var intersectionSimulation2 = IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights;
			jsonSimulationSetupRepository.Save(intersectionSimulation1);
			jsonSimulationSetupRepository.Save(intersectionSimulation2);

			// Act
			var result = jsonSimulationSetupRepository.LoadAll();

			// Assert
			result.IsError.Should().BeFalse();
			result.Value.Should().HaveCount(2);
			result.Value.Should().ContainEquivalentOf(intersectionSimulation1);
			result.Value.Should().ContainEquivalentOf(intersectionSimulation2);
		}
	}
}
