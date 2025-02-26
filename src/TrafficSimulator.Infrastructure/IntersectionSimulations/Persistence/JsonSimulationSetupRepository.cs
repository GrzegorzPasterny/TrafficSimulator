using CSharpFunctionalExtensions;
using ErrorOr;
using System.Text.Json;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.Common;
using TrafficSimulator.Infrastructure.DTOs;

namespace TrafficSimulator.Infrastructure.IntersectionSimulations.Persistence
{
	public class JsonSimulationSetupRepository : ISimulationSetupRepository
	{
		private readonly ISimulationSetupMapper _simulationSetupMapper;
		public JsonSimulationSetupRepositoryOptions Options { get; } = new();

		public JsonSimulationSetupRepository(ISimulationSetupMapper simulationSetupMapper)
		{
			_simulationSetupMapper = simulationSetupMapper;
		}

		public UnitResult<Error> Save(IntersectionSimulation intersectionSimulation)
		{
			ErrorOr<IntersectionSimulationDto> intersectionSimulationDtoResult = _simulationSetupMapper.ToDto(intersectionSimulation);

			if (intersectionSimulationDtoResult.IsError)
			{
				return intersectionSimulationDtoResult.FirstError;
			}

			try
			{
				string json = JsonSerializer.Serialize(intersectionSimulationDtoResult.Value, new JsonSerializerOptions { WriteIndented = true });

				string filePath = Path.Combine(Options.DirectoryPath, $"{intersectionSimulation.Name}_{intersectionSimulation.Id}.json");

				Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

				File.WriteAllText(filePath, json);

				return UnitResult.Success<Error>();
			}
			catch (Exception ex)
			{
				return InfrastructureErrors.JsonSaveFailure(ex.Message);
			}
		}

		public ErrorOr<IntersectionSimulation> Load(Guid id)
		{
			try
			{
				string filePattern = $"*_{id}.json";
				string[] files = Directory.GetFiles(Options.DirectoryPath, filePattern);

				if (files.Length == 0)
				{
					return Error.NotFound($"No simulation found with ID {id}");
				}

				string json = File.ReadAllText(files[0]);

				IntersectionSimulationDto? dto = JsonSerializer.Deserialize<IntersectionSimulationDto>(json);

				if (dto is null)
				{
					return Error.Failure("Deserialization failed: Simulation data is corrupted.");
				}

				return _simulationSetupMapper.ToDomain(dto);
			}
			catch (Exception ex)
			{
				return Error.Failure($"Failed to load simulation: {ex.Message}");
			}
		}

		public ErrorOr<List<IntersectionSimulation>> LoadAll()
		{
			try
			{
				if (!Directory.Exists(Options.DirectoryPath))
				{
					return new List<IntersectionSimulation>();
				}

				List<IntersectionSimulation> simulations = Directory
					.GetFiles(Options.DirectoryPath, "*.json")
					.Select(file =>
					{
						try
						{
							string json = File.ReadAllText(file);
							IntersectionSimulationDto? dto = JsonSerializer.Deserialize<IntersectionSimulationDto>(json);
							return dto is not null ? _simulationSetupMapper.ToDomain(dto) : Error.Failure($"Failed to parse {file}");
						}
						catch (Exception ex)
						{
							return Error.Failure($"Error reading {file}: {ex.Message}");
						}
					})
					.Where(result => result.IsError == false)
					.Select(result => result.Value)
					.ToList();

				return simulations;
			}
			catch (Exception ex)
			{
				// TODO: Create Infrastructure error
				return Error.Failure($"Failed to load all simulations: {ex.Message}");
			}
		}
	}
}
