﻿namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionSimulationOptionsDto
	{
		public double StepTimespanMs { get; set; }
		public double TimeoutMs { get; set; }
		public int StepLimit { get; set; }
		public string? TrafficLightHandlerType { get; set; }
	}
}
