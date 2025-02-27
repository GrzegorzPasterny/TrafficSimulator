using ConsoleAppFramework;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Presentation.Console.Controllers
{
	public class IntersectionSimulationController
	{
		private readonly ISimulationHandler _simulationHandler;
		private readonly ILogger<IntersectionSimulationController> _logger;

		public IntersectionSimulationController(ISimulationHandler simulationHandler, ILogger<IntersectionSimulationController> logger)
		{
			_simulationHandler = simulationHandler;
			_logger = logger;
		}

		/// <summary>Command to run intersection simulation</summary>
		/// <param name="simulationConfigurationFile">-f, Intersection simulation configuration file path</param>
		[Command("Run")]
		public async Task Run(string simulationConfigurationFile)
		{
			// TODO: Add Cancellation token source

			_logger.LogInformation("Starting simulation [ConfigurationFile = {ConfigurationFile}]", simulationConfigurationFile);

			var overallResult = await _simulationHandler.LoadIntersection(simulationConfigurationFile)
				.TapError(error => _logger.LogError("Intersection simulation loading failure [Error = {@Error}]", error))
				.Bind(async () => await _simulationHandler.Start()
					.TapError(error => _logger.LogError("Simulation failed [Error = {@Error}]", error))
				);

			if (overallResult.IsFailure)
			{
				_logger.LogInformation("Simulation finished with failure");
			}
			else
			{
				_logger.LogInformation("Simulation finished with success");
			}
		}
	}
}
