using System.Text;
using TrafficSimulator.Domain.Commons.Helpers;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.IntersectionProperties;

namespace TrafficSimulator.Domain.Models.Lights
{
	public class TrafficPhase
	{
		private Intersection _intersection;

		public TrafficPhase(string name, Intersection intersection)
		{
			Name = name;
			_intersection = intersection;

			TrafficLightsAssignments = _intersection.GetAllTurnDefinitionsWithTrafficLights().ToList();
		}

		public string Name { get; set; }
		public List<TurnWithTrafficLight> TrafficLightsAssignments { get; set; }
		public IEnumerable<TurnWithTrafficLight> LanesWithGreenLight
			=> TrafficLightsAssignments.Where(a => a.TrafficLightState is TrafficLightState.Green);
		public IEnumerable<TurnWithTrafficLight> LanesWithRedLight
			=> TrafficLightsAssignments.Where(a => a.TrafficLightState is TrafficLightState.Red);

		public void Apply()
		{
			foreach (var turn in TrafficLightsAssignments)
			{
				// TODO: Handle all of the cases below
				switch (turn.TrafficLightState)
				{
					case TrafficLightState.Off:
						break;
					case TrafficLightState.Green:
						turn.TurnPossibility.TrafficLights!.SwitchToGreen();
						break;
					case TrafficLightState.Orange:
						break;
					case TrafficLightState.Red:
						turn.TurnPossibility.TrafficLights!.SwitchToRed();
						break;
					case TrafficLightState.ConditionalRightGreen:
						break;
					default:
						break;
				}
			}
		}

		public void ApplyChange()
		{
			foreach (var turn in TrafficLightsAssignments)
			{
				// TODO: Handle all of the cases below
				switch (turn.TrafficLightState)
				{
					case TrafficLightState.Off:
						break;
					case TrafficLightState.Green:
						turn.TurnPossibility.TrafficLights!.SwitchToOrange();
						break;
					case TrafficLightState.Orange:
						break;
					case TrafficLightState.Red:
						break;
					case TrafficLightState.ConditionalRightGreen:
						break;
					default:
						break;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new();

			stringBuilder.Append($"[TrafficPhaseName = {Name}, ");

			foreach (var turn in TrafficLightsAssignments)
			{
				stringBuilder.Append(turn.ToString());
			}

			stringBuilder.Append(']');

			return stringBuilder.ToString();
		}
	}
}
