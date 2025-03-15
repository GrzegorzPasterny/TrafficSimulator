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
	}
}
