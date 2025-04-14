using FluentAssertions;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Infrastructure.AI;

namespace TrafficSimulator.Infrastructure.UnitTests.AI
{
	public class LearningAgentTests
	{
		[Fact]
		public void ShouldLoadOnnxModel()
		{
			IAiLearningAgent aiLearningAgent = new AiRlAgent("ML/model.zip");

			Action predictAction = () => aiLearningAgent.Predict([1, 0, 1000, 0, 0, 9, 0, 0]);

			predictAction.Should().NotThrow();
		}
	}
}
