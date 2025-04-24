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
				IntersectionsRepository.ForkFromWestAndEastThatMergesToNorthLane_NestSimulation(_mediator));

			simulationHandler.Start().GetAwaiter().GetResult();

			SimulationResults simulationResults = simulationHandler.SimulationResults!;

			return EvaluateFitness(simulationResults);
		}

		internal static FitnessInfo EvaluateFitness(SimulationResults simulationResults)
		{
			double reward = 0;

			if (simulationResults.TotalCars == simulationResults.CarsPassed)
			{
				reward += simulationResults.TotalCars * 1_000; // bonus for competing the simulation
			}

			reward += simulationResults.CarsPassed * 1_000; // reward for each car that passed the intersection

			reward -= simulationResults.TotalCarsIdleTimeMs; // punishment for each idle ms for each car

			reward -= simulationResults.SimulationStepsTaken * 10; // punishment for each simulation step

			Clamp(ref reward);

			return new FitnessInfo(reward);
		}

		private static void Clamp(ref double reward)
		{
			if (reward < 0)
			{
				reward = 0;
			}
		}
	}
}
