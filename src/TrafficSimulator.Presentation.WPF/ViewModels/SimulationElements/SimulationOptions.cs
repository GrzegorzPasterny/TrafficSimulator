using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Presentation.WPF.ViewModels.SimulationElements
{
    public class SimulationOptions
	{
		public static string DefaultSectionName => nameof(SimulationOptions);

		public string SimulationModeType { get; set; } = SimulationMode.RealTime;
		public string TrafficLightsMode { get; set; } = TrafficLightHandlerTypes.Default;
		public int CarSize { get; set; } = 15;
		public int CarGenerationAreaSize { get; set; } = 100;
	}
}
