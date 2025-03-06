namespace TrafficSimulator.Application.Simulation
{
	public class SimulationStateEventArgs : EventArgs
	{
		public int SimulationStep { get; }
		public string TrafficLightStatus { get; }
		//public List<CarData> Cars { get; }

		public SimulationStateEventArgs(int step, string trafficLightStatus)
		{
			SimulationStep = step;
			TrafficLightStatus = trafficLightStatus;
		}
	}
}
