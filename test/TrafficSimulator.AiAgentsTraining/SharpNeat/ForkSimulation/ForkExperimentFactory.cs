using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using TrafficSimulator.Application;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Domain;
using TrafficSimulator.Infrastructure.DI;
using Xunit.Abstractions;

namespace TrafficSimulator.AiAgentsTraining.SharpNeat.ForkSimulation
{
	public class ForkExperimentFactory
	{
		internal readonly ILogger<ForkExperimentFactory> _logger;
		internal readonly IMediator _mediator;
		internal readonly IntersectionSimulationHandlerFactory _simulationHandlerFactory;
		private readonly ISender _sender;

		public ForkExperimentFactory(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			var services = new ServiceCollection();
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();
			services.AddLogging(loggingBuilder =>
			{
				loggingBuilder.ClearProviders();
				loggingBuilder.AddSerilog(logger);
			});

			var provider = services.BuildServiceProvider();

			_logger = provider.GetRequiredService<ILogger<ForkExperimentFactory>>();
			_simulationHandlerFactory = provider.GetRequiredService<IntersectionSimulationHandlerFactory>();
			_sender = provider.GetRequiredService<ISender>();
		}

		[Fact]
		public void Train()
		{
			ExperimentConfig experimentConfig = ForkExperimentNeatConfig.GetConfig();

			var evalScheme = new ForkEvaluationScheme(_simulationHandlerFactory, _sender);

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

				if (ea.Population.Stats.BestFitness.PrimaryFitness >= ForkEvaluator.ExpectedBestFitnessLevel)
				{
					break;
				}
			}

			_logger.LogInformation("Number of generations: {Counter}", counter);
			_logger.LogInformation("Final score: {Fitness}", ea.Population.Stats.BestFitness.PrimaryFitness);

			ea.Population.Stats.BestFitness.PrimaryFitness.Should().BeGreaterOrEqualTo(ForkEvaluator.ExpectedBestFitnessLevel);
		}
	}
}
