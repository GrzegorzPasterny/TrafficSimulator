﻿using FluentAssertions;
using MediatR;
using Moq;
using Moq.Protected;
using TrafficSimulator.Domain.CarGenerators;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.UnitTests.CarGenerators
{
	public class WaveCarsGeneratorTests
	{
		Mock<Intersection> _rootMock = new("MockIntersection", null);
		Mock<IntersectionObject> _parentMock;
		Mock<ISender> _mediatorMock = new();
		private Mock<WaveCarsGenerator>? _waveCarsGeneratorMock;

		WaveCarsGeneratorOptions _options = new()
		{
			CarOptions = new CarOptions(),
			AmountOfCarsToGenerate = 1000,
			WavePeriodHz = 0.5, // 2s period
			WaveAmplitude = 30,
			BaseRate = 1
		};

		public WaveCarsGeneratorTests()
		{
			_parentMock = new Mock<IntersectionObject>(_rootMock.Object, _rootMock.Object, "MockParent");
		}

		[Theory]
		[InlineData(20000, 100, 10, 30)]
		[InlineData(20000, 500, 10, 30)]
		[InlineData(20000, 50, 10, 30)]
		[InlineData(20000, 25, 10, 30)]
		public async Task GenerateCars_WithHighCallFrequency_ShouldProduceCars(
			int overallGenerationTimeMs, int simulationStepMs, int expectedCarsMin, int expectedCarsMax)
		{
			_waveCarsGeneratorMock = new Mock<WaveCarsGenerator>(
				_rootMock.Object, _parentMock.Object, _mediatorMock.Object, _options)
			{
				CallBase = true
			};

			_waveCarsGeneratorMock.Protected()
				.Setup<Task>("GenerateCar")
				.Returns(Task.CompletedTask);

			WaveCarsGenerator waveCarsGenerator = _waveCarsGeneratorMock.Object;

			int simulationSteps = overallGenerationTimeMs / simulationStepMs;

			for (int i = 0; i < simulationSteps; i++)
			{
				await waveCarsGenerator.Generate(TimeSpan.FromMilliseconds(simulationStepMs));
			}

			waveCarsGenerator.IsGenerationCompleted.Should().BeFalse();
			_waveCarsGeneratorMock.Protected()
				.Verify("GenerateCar", Times.Between(expectedCarsMin, expectedCarsMax, Moq.Range.Inclusive));
		}
	}
}
