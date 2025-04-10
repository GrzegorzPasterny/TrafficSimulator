using Microsoft.ML.Data;

namespace TrafficSimulator.Domain.AI
{
	public class TrafficState
	{
		[VectorType(8)]
		public float[] Inputs { get; set; } = new float[8];

		[VectorType(4)]
		public float[] QValues { get; set; } = new float[4];

		[ColumnName("Label")]
		public float Reward { get; set; }
	}
}
