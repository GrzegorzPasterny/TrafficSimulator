using TrafficSimulator.Domain.AI;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface IAiLearningAgent : IAiAgent
	{
		public void TrainModel();
		public void CollectTrainingData(TrafficState state);


	}
}
