using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Presentation.WPF.ViewModels.Items;

namespace TrafficSimulator.Presentation.WPF.Extensions
{
    internal static class TrafficPhaseExtensions
	{
		public static TrafficPhaseItem ToTrafficPhaseItem(this TrafficPhase trafficPhase)
		{
			return new TrafficPhaseItem()
			{
				Name = trafficPhase.Name
			};
		}

		public static TrafficLightsModeItem ToTrafficPhaseModeItem(this string? trafficPhaseMode)
		{
			if (trafficPhaseMode is null)
			{
				return new TrafficLightsModeItem() { Name = TrafficLightHandlerTypes.Default };
			}

			if (!TrafficLightHandlerTypes.Modes.Contains(trafficPhaseMode))
			{
				// TODO: Handle
				throw new ArgumentOutOfRangeException();
			}

			return new TrafficLightsModeItem() { Name = trafficPhaseMode };
		}
	}
}
