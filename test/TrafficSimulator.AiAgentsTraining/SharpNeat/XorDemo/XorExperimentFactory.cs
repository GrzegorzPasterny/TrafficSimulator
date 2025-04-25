using FluentAssertions;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using Xunit.Abstractions;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.XorDemo;

public class XorExperimentFactory
{
	private readonly ITestOutputHelper _output;

	public XorExperimentFactory(ITestOutputHelper output)
	{
		_output = output;
	}

	[Fact]
	public void Evolve_XOR_And_SaveLoadBestGenome()
	{
		ExperimentConfig experimentConfig = new()
		{
			Name = "XOR",
			IsAcyclic = true,
			ActivationFnName = nameof(LeakyReLU),
			PopulationSize = 100,
			InitialInterconnectionsProportion = 0.05,
			ConnectionWeightScale = 5,
			DegreeOfParallelism = 4,
			EnableHardwareAcceleratedActivationFunctions = false,
			EnableHardwareAcceleratedNeuralNets = false,
			EvolutionAlgorithm = new NeatEvolutionAlgorithmConfig(),
			ReproductionAsexual = new NeatReproductionAsexualConfig(),
			ComplexityRegulationStrategy = new ComplexityRegulationStrategyConfig() { StrategyName = "null" },
			Description = "Test",
			Id = "1234",
			ReproductionSexual = new NeatReproductionSexualConfig(),
		};

		// Create an evaluation scheme object for the XOR task.
		var evalScheme = new XorEvaluationScheme<double>();

		var experiment = new NeatExperiment<double>(evalScheme, "XOR");
		experiment.Configure(experimentConfig);

		NeatEvolutionAlgorithm<double> ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment);

		ea.Initialise();

		var neatPop = ea.Population;

		int counter = 0;

		while (counter < 1_000)
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
