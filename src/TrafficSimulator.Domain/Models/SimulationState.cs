using System.Text;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Domain.Models
{
	public class SimulationState
	{
		public SimulationPhase SimulationPhase { set; get; } = SimulationPhase.NotStarted;
		public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;
		public int StepsCount { get; set; } = 0;

		public List<Car> Cars { get; set; } = [];
		public List<ICarGenerator> CarGenerators { get; set; } = [];

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"Simulation State: ({SimulationPhase})");
			stringBuilder.AppendLine($"[Step = {StepsCount}, TimeElapsed = {ElapsedTime}]");

			foreach (ICarGenerator carGenerator in CarGenerators)
			{
				stringBuilder.AppendLine(carGenerator.ToString());
			}

			foreach (Car car in Cars)
			{
				stringBuilder.AppendLine(car.ToString());
			}

			return stringBuilder.ToString();
		}
	}
}
