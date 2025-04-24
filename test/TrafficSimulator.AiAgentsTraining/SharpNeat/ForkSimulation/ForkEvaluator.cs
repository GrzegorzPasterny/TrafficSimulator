using SharpNeat;
using SharpNeat.Evaluation;
using System.Numerics;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluator<TScalar> : IPhenomeEvaluator<IBlackBox<TScalar>>
	where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
	{
		public const double ExpectedBestFitnessLevel = 100;

		public FitnessInfo Evaluate(IBlackBox<TScalar> phenome)
		{
			throw new NotImplementedException();
		}
	}
}
