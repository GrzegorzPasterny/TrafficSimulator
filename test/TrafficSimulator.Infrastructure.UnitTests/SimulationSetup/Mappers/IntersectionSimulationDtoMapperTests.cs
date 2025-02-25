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
		public void MapFromDomainToDto_GivenZebraCrossing_ShouldProduceResultsAsExpected()
		{
			// Arrange
			IntersectionSimulationDtoMapper mapper = new IntersectionSimulationDtoMapper();

			IntersectionSimulation intersectionSimulation = new(IntersectionsRepository.ZebraCrossingOnOneLaneRoadEastWest);

			ErrorOr<IntersectionSimulationDto> intersectionSimulationDto = mapper.ToDto(intersectionSimulation);

			intersectionSimulationDto.IsError.Should().BeFalse();
			intersectionSimulationDto.Value.Should().BeEquivalentTo(IntersectionSimulationDtosRepository.ZebraCrossingOnOneLaneRoadEastWest);
		}
	}
}
