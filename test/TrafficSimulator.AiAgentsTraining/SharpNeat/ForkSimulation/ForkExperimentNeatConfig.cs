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
				PopulationSize = 10, // default: more than 150
				InitialInterconnectionsProportion = 0.05,
				ConnectionWeightScale = 5,
				DegreeOfParallelism = 1, // default: 4
				EnableHardwareAcceleratedActivationFunctions = false,
				EnableHardwareAcceleratedNeuralNets = false,
				Description = "Fork intersection simulation powered by NEAT",
				Id = "1234",
				EvolutionAlgorithm = new NeatEvolutionAlgorithmConfig()
				{
					SpeciesCount = 4, // default: 10
					ElitismProportion = 0.2,
					SelectionProportion = 0.2,
					OffspringAsexualProportion = 0.5,
					OffspringSexualProportion = 0.5,
					InterspeciesMatingProportion = 0.01,
					StatisticsMovingAverageHistoryLength = 100
				},
				ReproductionAsexual = new NeatReproductionAsexualConfig()
				{
					ConnectionWeightMutationProbability = 0.94,
					AddNodeMutationProbability = 0.01,
					AddConnectionMutationProbability = 0.025,
					DeleteConnectionMutationProbability = 0.025
				},
				ComplexityRegulationStrategy = new ComplexityRegulationStrategyConfig()
				{
					StrategyName = "absolute", // absolute, relative, null
					ComplexityCeiling = 20, // unknown default
					MinSimplifcationGenerations = 10,
					RelativeComplexityCeiling = 30,
				},
				ReproductionSexual = new NeatReproductionSexualConfig()
				{
					SecondaryParentGeneProbability = 0.1
				},
			};
		}
	}
}
