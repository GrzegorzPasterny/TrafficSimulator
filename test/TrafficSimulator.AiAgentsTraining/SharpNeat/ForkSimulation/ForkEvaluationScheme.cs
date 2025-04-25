using MediatR;
using SharpNeat;
using SharpNeat.Evaluation;
using TrafficSimulator.Application.Simulation;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluationScheme : IBlackBoxEvaluationScheme<double>
	{
		private IntersectionSimulationHandlerFactory _simulationHandlerFactory;
		private readonly ISender _mediator;

		public ForkEvaluationScheme(IntersectionSimulationHandlerFactory simulationHandlerFactory, ISender mediator)
		{
			_simulationHandlerFactory = simulationHandlerFactory;
			_mediator = mediator;
		}

		public int InputCount => 9;

		public int OutputCount => 2;

		public bool IsDeterministic => true;

		public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

		public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

		public bool EvaluatorsHaveState => false;

		public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
		{
			return new ForkEvaluator(_simulationHandlerFactory, _mediator);
		}

		public bool TestForStopCondition(FitnessInfo fitnessInfo)
		{
			return fitnessInfo.PrimaryFitness >= ForkEvaluator.ExpectedBestFitnessLevel;
		}
	}
}
