using System.Text;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Domain.Models
{
	public class SimulationState
	{
		public SimulationPhase SimulationPhase { set; get; }

		public List<Car> Cars { get; set; } = new();
		public List<ICarGenerator> CarGenerators { get; set; } = new();

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"Simulation State: ({SimulationPhase})");

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
