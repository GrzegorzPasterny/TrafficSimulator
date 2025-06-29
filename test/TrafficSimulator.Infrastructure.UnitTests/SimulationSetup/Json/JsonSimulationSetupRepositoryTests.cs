﻿using FluentAssertions;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.IntersectionSimulations;
using TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.Infrastructure.UnitTests.SimulationSetup.Json
{
	public class JsonSimulationSetupRepositoryTests
	{
		public static IEnumerable<object[]> SaveIntersectionSimulationToJson_ShouldProduceFileOnTheDisc_DataSource()
		{
			yield return new object[]
			{
				IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest
			};
			yield return new object[]
			{
				IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWestWithCarGenerators(null)
			};
			yield return new object[]
			{
				IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLights
			};
			yield return new object[]
			{
				IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLightsWithMultipleCarGenerators(null)
			};
			yield return new object[]
			{
				IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLane_NestSimulation(null, null)
			};
			yield return new object[]
			{
				IntersectionsRepository.ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLights
			};
			yield return new object[]
			{
				IntersectionsRepository.ThreeDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(null)
			};
			yield return new object[]
			{
				IntersectionsRepository.FourDirectionalEastSouthWestWithInboundAndOutboundLanesWithTrafficLightsWithCarGenerators(null)
			};
			yield return new object[]
			{
				IntersectionsRepository.FourDirectional_Full(null)
			};
			yield return new object[]
			{
				IntersectionsRepository.FourDirectional_2Lanes_Full(null)
			};
		}

		[Theory]
		[MemberData(nameof(SaveIntersectionSimulationToJson_ShouldProduceFileOnTheDisc_DataSource))]
		public void SaveIntersectionSimulationToJson_ShouldProduceFileOnTheDisc(IntersectionSimulation intersectionSimulation)
		{
			// Arrange
			CarGeneratorFactory carGeneratorFactory = new();

			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new(new IntersectionSimulationDtoMapper(carGeneratorFactory));

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
			var intersectionSimulation = IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest;
			CarGeneratorFactory carGeneratorFactory = new();

			JsonSimulationSetupRepository jsonSimulationSetupRepository =
				new(new IntersectionSimulationDtoMapper(carGeneratorFactory));

			// Save a test simulation to the file system
			jsonSimulationSetupRepository.Save(intersectionSimulation);

			// Act
			var result = jsonSimulationSetupRepository.Load(intersectionSimulation.Id);

			// Assert
			result.IsError.Should().BeFalse();
		}
	}
}
