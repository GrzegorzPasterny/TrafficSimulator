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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

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
