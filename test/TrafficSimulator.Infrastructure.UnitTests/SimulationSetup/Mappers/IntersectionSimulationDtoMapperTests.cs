using ErrorOr;
using FluentAssertions;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DTOs;
using TrafficSimulator.Infrastructure.IntersectionSimulations;
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

			IntersectionSimulation intersectionSimulation = new(IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest);

			// Act
			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto = mapper.ToDto(intersectionSimulation);

			// Assert
			intersectionSimulationDto.IsError.Should().BeFalse();
			intersectionSimulationDto.Value.Should().BeEquivalentTo(IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest);
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
			intersectionSimulation.Value.Should().BeEquivalentTo(IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest);
		}
	}
}
