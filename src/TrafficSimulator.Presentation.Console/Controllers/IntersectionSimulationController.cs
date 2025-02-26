using ConsoleAppFramework;
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
		[Command("RunSimulation")]
		public void Run(string simulationConfigurationFile)
		{
			_logger.LogInformation("Starting simulation [ConfigurationFile = {ConfigurationFile}", simulationConfigurationFile);

			//_simulationHandler.LoadIntersection()
		}

		/// <summary>Display message.</summary>
		/// <param name="msg">Message to show.</param>
		//public void Echo(string msg) => System.Console.WriteLine(msg);

		/// <summary>Sum parameters.</summary>
		/// <param name="x">left value.</param>
		/// <param name="y">right value.</param>
		//public void Sum(int x, int y) => System.Console.WriteLine(x + y);
	}
}
