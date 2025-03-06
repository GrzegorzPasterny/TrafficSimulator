using System.Windows.Media;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Presentation.WPF.Extensions
{
	public static class TrafficLightsStateExtensions
	{
		public static SolidColorBrush ToColor(this TrafficLightState trafficLightState)
		{
			switch (trafficLightState)
			{
				case TrafficLightState.Off:
					return Brushes.Gray;
				case TrafficLightState.Green:
					return Brushes.Green;
				case TrafficLightState.Orange:
					return Brushes.Orange;
				case TrafficLightState.Red:
					return Brushes.Red;
				case TrafficLightState.ConditionalRightGreen:
					return Brushes.LightGreen;
				default:
					// TODO: Handle
					throw new ArgumentException();
			}
		}
	}
}
