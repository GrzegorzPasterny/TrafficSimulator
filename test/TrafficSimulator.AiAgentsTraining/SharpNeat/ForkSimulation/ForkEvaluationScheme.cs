using SharpNeat.Evaluation;
using System.Numerics;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluationScheme<TScalar> : IBlackBoxEvaluationScheme<TScalar> where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
	{
		public int InputCount => throw new NotImplementedException();

		public int OutputCount => throw new NotImplementedException();

		public bool IsDeterministic => throw new NotImplementedException();

		public IComparer<FitnessInfo> FitnessComparer => throw new NotImplementedException();

		public FitnessInfo NullFitness => throw new NotImplementedException();

		public bool EvaluatorsHaveState => throw new NotImplementedException();

		public IPhenomeEvaluator<global::SharpNeat.IBlackBox<TScalar>> CreateEvaluator()
		{
			throw new NotImplementedException();
		}

		public bool TestForStopCondition(FitnessInfo fitnessInfo)
		{
			throw new NotImplementedException();
		}
	}
}
