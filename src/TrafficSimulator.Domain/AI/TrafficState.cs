namespace TrafficSimulator.Domain.AI
{
	public class TrafficState
	{
		[VectorType(?)] public float[] Inputs { get; set; }
		[VectorType(?)] public float[] QValues { get; set; }
		public float Reward { get; set; }
	}
}
