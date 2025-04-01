namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface IAiAgent
	{
		IReadOnlyList<float> Predict(IEnumerable<float> input);
	}
}
