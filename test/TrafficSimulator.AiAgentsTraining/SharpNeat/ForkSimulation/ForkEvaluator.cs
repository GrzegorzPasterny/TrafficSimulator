using MediatR;
using SharpNeat;
using SharpNeat.Evaluation;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Tests.Commons.Assets;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluator : IPhenomeEvaluator<IBlackBox<double>>
	{
		public const double ExpectedBestFitnessLevel = 100;
		private IntersectionSimulationHandlerFactory _simulationHandlerFactory;
		private readonly ISender _mediator;

		public ForkEvaluator(IntersectionSimulationHandlerFactory simulationHandlerFactory, ISender mediator)
		{
			_simulationHandlerFactory = simulationHandlerFactory;
			_mediator = mediator;
		}

		public FitnessInfo Evaluate(IBlackBox<double> phenome)
		{
			ISimulationHandler simulationHandler = _simulationHandlerFactory.CreateHandler(SimulationMode.InMemory);

			simulationHandler.LoadIntersection(
				IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLaneWithTrafficLightsWithMultipleCarGenerators(_mediator));

			simulationHandler.Start().GetAwaiter().GetResult();

			SimulationResults simulationResults = simulationHandler.SimulationResults!;

			return EvaluateFitness(simulationResults);
		}

		private FitnessInfo EvaluateFitness(SimulationResults simulationResults)
		{
			throw new NotImplementedException();
		}
	}
}
