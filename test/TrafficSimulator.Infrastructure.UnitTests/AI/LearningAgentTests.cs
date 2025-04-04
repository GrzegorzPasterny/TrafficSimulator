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
			IAiLearningAgent aiLearningAgent = new AiAgent("Assets/current.onnx");

			Action predictAction = () => aiLearningAgent.Predict([0, 0, 0, 0, 0, 0, 0, 0]);

			predictAction.Should().NotThrow();
		}
	}
}
