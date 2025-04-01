namespace TrafficSimulator.Domain.AI
{
	public class TrafficState
	{
		public float[] Inputs { get; set; } // Example: [Cars in Lane 1, Cars in Lane 2, Current Light Phase, ...]
		public float[] QValues { get; set; } // Predicted Q-values for actions
	}
}
