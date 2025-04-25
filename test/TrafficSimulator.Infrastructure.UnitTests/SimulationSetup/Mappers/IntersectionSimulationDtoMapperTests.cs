using ErrorOr;
using FluentAssertions;
using System.Text.Json;
using TrafficSimulator.Domain.CarGenerators;
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
			CarGeneratorFactory carGeneratorFactory = new();

			IntersectionSimulationDtoMapper mapper = new(carGeneratorFactory);

			IntersectionSimulation intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators(null);

			// Act
			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto = mapper.ToDto(intersectionSimulation);

			// Assert
			intersectionSimulationDto.IsError.Should().BeFalse();
			intersectionSimulationDto.Value.Should().BeEquivalentTo(IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators,
				options => options
					.Using<JsonElement>(ctx => ctx.Subject.ToString().Should().Be(ctx.Expectation.ToString())) // Compare JSON content
					.WhenTypeIs<JsonElement>());
		}


		public static IEnumerable<object[]> MapFromDomainToDtoAndBackToDomain_ShouldProduceDtosAsExpected_DataSource()
		{
			yield return new object[]
			{
				IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators(null),
			};
			yield return new object[]
			{
				IntersectionsRepository.FourDirectional_Full(null),
			};
			yield return new object[]
			{
				IntersectionsRepository.FourDirectional_2Lanes_Full(null),
			};
		}

		[Theory]
		[MemberData(nameof(MapFromDomainToDtoAndBackToDomain_ShouldProduceDtosAsExpected_DataSource))]
		public void MapFromDomainToDtoAndBackToDomain_ShouldProduceDtosAsExpected(
			IntersectionSimulation intersectionSimulation)
		{
			// Arrange
			CarGeneratorFactory carGeneratorFactory = new();

			IntersectionSimulationDtoMapper mapper = new(carGeneratorFactory);

			// Act
			ErrorOr<IntersectionSimulationDto> intersectionSimulationDtoResult = mapper.ToDto(intersectionSimulation);

			// Assert
			intersectionSimulationDtoResult.IsError.Should().BeFalse();

			// Act
			ErrorOr<IntersectionSimulation> intersectionSimulationActualResult = mapper.ToDomain(intersectionSimulationDtoResult.Value);

			// Assert
			intersectionSimulationActualResult.IsError.Should().BeFalse();
			intersectionSimulationActualResult.Value.Should().BeEquivalentTo(intersectionSimulation);

		}

		public static IEnumerable<object[]> MapFromDtoToDomain_GivenZebraCrossingDto_ShouldProduceDomainObjectsAsExpected_DataSource()
		{
			yield return new object[]
			{
				IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest,
				IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest
			};
		}

		[Theory]
		[MemberData(nameof(MapFromDtoToDomain_GivenZebraCrossingDto_ShouldProduceDomainObjectsAsExpected_DataSource))]
		public void MapFromDtoToDomain_GivenZebraCrossingDto_ShouldProduceDomainObjectsAsExpected(
			IntersectionSimulation intersectionSimulationExpected, IntersectionSimulationDto intersectionSimulationDto)
		{
			// Arrange
			CarGeneratorFactory carGeneratorFactory = new();

			IntersectionSimulationDtoMapper mapper = new(carGeneratorFactory);

			// Act
			ErrorOr<IntersectionSimulation> intersectionSimulation = mapper.ToDomain(intersectionSimulationDto);

			// Assert
			intersectionSimulation.IsError.Should().BeFalse();

			intersectionSimulation.Value.Should().BeEquivalentTo(intersectionSimulationExpected);
		}

		[Fact]
		public void MapFromDtoToDomainAndBackToDto_GivenZebraCrossingDto_DtosObjectsShouldBeTheSame()
		{
			// Arrange
			CarGeneratorFactory carGeneratorFactory = new();
			IntersectionSimulationDtoMapper mapper = new(carGeneratorFactory);
			IntersectionSimulationDto intersectionSimulationDto = IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest;

			// Act
			ErrorOr<IntersectionSimulation> intersectionSimulation = mapper.ToDomain(intersectionSimulationDto);

			intersectionSimulation.IsError.Should().BeFalse();

			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto2Result = mapper.ToDto(intersectionSimulation.Value);

			// Assert
			intersectionSimulationDto2Result.IsError.Should().BeFalse();
			intersectionSimulationDto2Result.Value.Should().BeEquivalentTo(intersectionSimulationDto,
				options => options
					.Using<JsonElement>(ctx => ctx.Subject.ToString().Should().Be(ctx.Expectation.ToString())) // Compare JSON content
					.WhenTypeIs<JsonElement>());
		}

		[Fact]
		public void LoadAllIntersectionSimulations_ShouldReturnListOfSimulations_WhenFilesExist()
		{
			// Arrange
			CarGeneratorFactory carGeneratorFactory = new();
			IntersectionSimulationDtoMapper mapper = new(carGeneratorFactory);
			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new(mapper);

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
