using FluentAssertions;
using SharpNeat.Evaluation;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluatorTests
	{
		[Theory]
		[InlineData(1, 1, 10, 0.0, 1900)]
		[InlineData(1, 0, 100, 5000.0, 0)]
		[InlineData(10, 8, 50, 5000.0, 2500)]
		[InlineData(10, 10, 65, 7000.0, 12350)]
		[InlineData(10, 10, 65, 12000.0, 7350)]
		[InlineData(100, 50, 200, 40000.0, 8000)]
		[InlineData(100, 100, 150, 40000.0, 158500)]
		[InlineData(100, 100, 250, 80000.0, 117500)]
		public void EvaluateFitness(int totalCars, int carsPassed, int simulationStepsTaken, double totalCarsIdleTimeMs, double fitnessExpected)
		{
			SimulationResults simulationResults = new SimulationResults()
			{
				TotalCars = totalCars,
				CarsPassed = carsPassed,
				SimulationStepsTaken = simulationStepsTaken,
				TotalCarsIdleTimeMs = totalCarsIdleTimeMs
			};

			FitnessInfo fitnessInfo = ForkEvaluator.EvaluateFitness(simulationResults);

			fitnessInfo.PrimaryFitness.Should().Be(fitnessExpected);
		}
	}
}
