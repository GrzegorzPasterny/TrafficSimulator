using SharpNeat;
using SharpNeat.Evaluation;
using System.Numerics;

namespace TrafficSimulator.AiAgentsTraining.SharpNeatDemo
{
	public sealed class XorEvaluationScheme<TScalar> : IBlackBoxEvaluationScheme<TScalar> where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
	{
		public int InputCount => 3;

		public int OutputCount => 1;

		public bool IsDeterministic => true;

		public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

		public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

		public bool EvaluatorsHaveState => false;

		public IPhenomeEvaluator<IBlackBox<TScalar>> CreateEvaluator()
		{
			return new XorEvaluator<TScalar>();
		}

		public bool TestForStopCondition(FitnessInfo fitnessInfo)
		{
			return (fitnessInfo.PrimaryFitness >= 10);
		}
	}
}
