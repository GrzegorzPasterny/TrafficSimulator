using SharpNeat.Experiments.ConfigModels;
using SharpNeat.NeuralNets.Double.ActivationFunctions;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public static class ForkExperimentNeatConfig
	{
		public static ExperimentConfig GetConfig()
		{
			return new ExperimentConfig()
			{
				Name = "ForkIntersection",
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
				ReproductionSexual = new NeatReproductionSexualConfig(),
				Description = "Fork intersection simulation powered by NEAT",
				Id = "1234",
			};
		}
	}
}
