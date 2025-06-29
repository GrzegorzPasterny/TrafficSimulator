﻿using SharpNeat;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation.Snapshots;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class Intersection : IntersectionObject, IEquatable<Intersection>
	{
		private IBlackBox<double>? _blackBox;

		public List<IntersectionObject> ObjectLookup { get; } = new();
		public List<Lanes> LanesCollection { get; set; } = new List<Lanes>();

		public IntersectionCore? IntersectionCore { get; set; }

		public List<TrafficPhase> TrafficPhases { get; } = [];

		public Intersection(string name, IntersectionObject? parent = null) : base(null!, parent, name)
		{
		}

		public void AddObject(IntersectionObject obj)
		{
			ObjectLookup.Add(obj);
		}

		public T? GetObject<T>(string fullName) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(o => o.FullName == fullName);
		}

		public T? GetObject<T>(Func<T, bool> predicate) where T : IntersectionObject
		{
			return ObjectLookup.OfType<T>().FirstOrDefault(predicate);
		}

		public override bool Equals(object? obj)
		{
			return obj is Intersection other && Equals(other);
		}

		public bool Equals(Intersection? other)
		{
			if (other == null) return false;

			return base.Equals(other);
		}

		public List<TrafficLightsSnapshot> CreateTrafficLightsSnapshots()
		{
			return ObjectLookup
				.OfType<TrafficLight>()
				.Select(trafficLight => new TrafficLightsSnapshot()
				{
					Id = trafficLight.Id,
					Name = trafficLight.FullName,
					TrafficLightState = trafficLight.TrafficLightState,
					InboundLaneName = trafficLight.Parent.FullName
				})
				.ToList();
		}

		// TODO: I do not like this method being here.
		// It should be somehow injected to the traffic lights handler that needs it.
		public void LoadNestModel(IBlackBox<double> blackBox)
		{
			_blackBox = blackBox;
		}

		public IBlackBox<double>? GetNestModel()
		{
			return _blackBox;
		}
	}
}
