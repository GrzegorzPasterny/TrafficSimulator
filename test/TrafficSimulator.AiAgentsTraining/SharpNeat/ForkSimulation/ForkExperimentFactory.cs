using FluentAssertions;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using Xunit.Abstractions;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkExperimentFactory
	{
		private readonly ITestOutputHelper _output;

		public ForkExperimentFactory(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void Train()
		{
			ExperimentConfig experimentConfig = ForkExperimentNeatConfig.GetConfig();

			var evalScheme = new ForkEvaluationScheme<double>();

			var experiment = new NeatExperiment<double>(evalScheme, "FORK");
			experiment.Configure(experimentConfig);

			NeatEvolutionAlgorithm<double> ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment);
			ea.Initialise();

			var neatPop = ea.Population;
			int counter = 0;

			while (counter < 100)
			{
				counter++;
				ea.PerformOneGeneration();

				if (ea.Population.Stats.BestFitness.PrimaryFitness >= 10)
				{
					break;
				}
			}

			_output.WriteLine($"Number of generations: {counter}");
			_output.WriteLine($"Final score: {ea.Population.Stats.BestFitness.PrimaryFitness}");

			ea.Population.Stats.BestFitness.PrimaryFitness.Should().BeGreaterOrEqualTo(10);
		}
	}
}
