using FluentAssertions;
using SharpNeat.Evaluation;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkEvaluatorTests
	{
		[Theory]
		[InlineData(1, 1, 10, 0, 101_900)]
		[InlineData(1, 0, 100, 5000, 94_000)]
		[InlineData(10, 8, 50, 5000, 102_500)]
		[InlineData(10, 10, 65, 7000, 112_350)]
		[InlineData(10, 10, 65, 12000, 107_350)]
		[InlineData(100, 50, 200, 40000, 108_000)]
		[InlineData(100, 100, 150, 40000, 258_500)]
		[InlineData(100, 100, 250, 80000, 217_500)]
		[InlineData(18, 18, 176, 11320, 122_920)]
		[InlineData(18, 18, 192, 15040, 119_040)]
		[InlineData(18, 8, 250, 72000, 33_500)]
		public void EvaluateFitness(int totalCars, int carsPassed, int simulationStepsTaken, double totalCarsIdleTimeMs, double fitnessExpected)
		{
			SimulationResults simulationResults = new()
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
