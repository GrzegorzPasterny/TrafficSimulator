using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using TrafficSimulator.Application.Commons.Interfaces;

namespace TrafficSimulator.Presentation.Console.Controllers
{
	public class IntersectionSimulationController
	{
		private readonly ISimulationHandler _simulationHandler;
		private readonly ISimulationSetupRepository _simulationSetupRepository;
		private readonly ILogger<IntersectionSimulationController> _logger;

		public IntersectionSimulationController(ISimulationHandler simulationHandler, ISimulationSetupRepository simulationSetupRepository, ILogger<IntersectionSimulationController> logger)
		{
			_simulationHandler = simulationHandler;
			_simulationSetupRepository = simulationSetupRepository;
			_logger = logger;
		}

		/// <summary>Command to run intersection simulation</summary>
		/// <param name="simulationConfigurationFile">-f, Intersection simulation configuration file path</param>
		[Command("Run")]
		public void Run(string simulationConfigurationFile)
		{
			_logger.LogInformation("Starting simulation [ConfigurationFile = {ConfigurationFile}]", simulationConfigurationFile);

			_simulationHandler.LoadIntersection(simulationConfigurationFile);
		}

	}
}
