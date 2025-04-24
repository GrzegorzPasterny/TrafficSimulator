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

		public int InputCount => throw new NotImplementedException();

		public int OutputCount => throw new NotImplementedException();

		public bool IsDeterministic => throw new NotImplementedException();

		public IComparer<FitnessInfo> FitnessComparer => throw new NotImplementedException();

		public FitnessInfo NullFitness => throw new NotImplementedException();

		public bool EvaluatorsHaveState => throw new NotImplementedException();

		public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
		{
			return new ForkEvaluator(_simulationHandlerFactory, _mediator);
		}

		public bool TestForStopCondition(FitnessInfo fitnessInfo)
		{
			throw new NotImplementedException();
		}
	}
}
