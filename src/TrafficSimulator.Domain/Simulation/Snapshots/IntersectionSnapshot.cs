﻿namespace TrafficSimulator.Domain.Simulation.Snapshots
{
	public class IntersectionSnapshot
	{
		public List<TrafficLightsSnapshot> TrafficLightsSnapshots { get; set; } = [];
		public List<CarSnapshot> CarSnapshots { get; set; } = [];

	}
}
