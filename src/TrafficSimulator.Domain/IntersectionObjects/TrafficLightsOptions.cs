﻿using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class TrafficLightsOptions
	{
		public TrafficLightState InitialState { get; set; } = TrafficLightState.Red;
		public TimeSpan SwitchLightTimespanMs { get; set; } = TimeSpan.FromSeconds(2);
	}
}
