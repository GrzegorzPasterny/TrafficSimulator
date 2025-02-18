using TrafficSimulator.Domain.Models.TrafficLights;

namespace TrafficSimulator.Application.Handlers.TrafficPhases
{
	public class TrafficPhasesHandler
	{
		public List<TrafficPhase> TrafficPhases { get; set; } = [];

		public void SetPhase(TrafficPhase phase)
		{
			// TODO: Check if phase can be found int TrafficPhases
		}
	}
}
