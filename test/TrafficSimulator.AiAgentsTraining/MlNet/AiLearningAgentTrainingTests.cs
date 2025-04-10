using CSharpFunctionalExtensions;
using ErrorOr;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TrafficSimulator.Application;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Domain;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Infrastructure.DI;
using TrafficSimulator.Tests.Commons.Assets;
using Xunit.Abstractions;

namespace TrafficSimulator.AiAgentsTraining.MlNet
{
	public class AiLearningAgentTrainingTests
	{
		internal readonly ILogger<AiLearningAgentTrainingTests> _logger;
		internal readonly IMediator _mediator;
		internal readonly IntersectionSimulationHandlerFactory _intersectionSimulationHandlerFactory;

		public AiLearningAgentTrainingTests(ITestOutputHelper testOutputHelper)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.TestOutput(testOutputHelper, outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.WriteTo.File("Logs/log.log", outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			var services = new ServiceCollection();
			services.AddDomain();
			services.AddApplication();
			services.AddInfrastructure();
			services.AddLogging((loggingBuilder) =>
			{
				loggingBuilder.ClearProviders();
				loggingBuilder.AddSerilog(logger);
			});
			var provider = services.BuildServiceProvider();
			_mediator = provider.GetRequiredService<IMediator>();

			_logger = provider.GetRequiredService<ILogger<AiLearningAgentTrainingTests>>();
			_intersectionSimulationHandlerFactory = provider.GetRequiredService<IntersectionSimulationHandlerFactory>();
		}

		[Fact]
		public async Task TrainAiModel_OnFourLaneIntersection_ShouldProduceFileWithTrainedModel()
		{
			// Arrange
			IntersectionSimulation intersectionSimulation =
				IntersectionsRepository.FourDirectional_Full_AiLearning(_mediator);

			using ISimulationHandler simulationHandler =
				_intersectionSimulationHandlerFactory.CreateHandler(SimulationMode.InMemory);
			simulationHandler.LoadIntersection(intersectionSimulation).IsSuccess.Should().BeTrue();

			UnitResult<Error> simulationStartResult = await simulationHandler.Start();

			simulationStartResult.IsSuccess.Should().BeTrue();
			simulationHandler.SimulationState.SimulationPhase.Should()
				.Match<SimulationPhase>(phase => phase == SimulationPhase.Finished || phase == SimulationPhase.Aborted);
		}
	}
}
