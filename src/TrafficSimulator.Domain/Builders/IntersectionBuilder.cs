﻿using ErrorOr;
using TrafficSimulator.Domain.Commons.Interfaces;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;

namespace TrafficSimulator.Domain.Commons.Builders
{
	public class IntersectionBuilder
	{
		private Intersection _intersection;

		private IntersectionBuilder(Intersection intersection)
		{
			_intersection = intersection;
		}

		public static IntersectionBuilder Create(string name = "")
		{
			Intersection intersection = new(name);

			IntersectionBuilder builder = new(intersection);

			return builder;
		}

		public IntersectionBuilder AddIntersectionCore(string name = "", int distance = 10)
		{
			_intersection.IntersectionCore = new IntersectionCore(_intersection, _intersection, name, distance);

			return this;
		}

		public IntersectionBuilder AddLanesCollection(WorldDirection worldDirection, string name = "")
		{
			Lanes lanes = new(_intersection, _intersection, worldDirection, name);

			// TODO: Check if there is already lanes object with the same world direction
			// TODO: Shuold throw an exception?

			_intersection.LanesCollection.Add(lanes);

			return this;
		}

		public IntersectionBuilder AddOutboundLane(WorldDirection worldDirection, string name = "", int distance = 100)
		{
			Lanes lanesCollection = _intersection.LanesCollection.Single(l => l.WorldDirection == worldDirection);

			OutboundLane outboundLane = new(_intersection, lanesCollection, worldDirection, name)
			{
				Distance = distance,
			};

			lanesCollection.OutboundLanes!.Add(outboundLane);

			return this;
		}

		public IntersectionBuilder AddInboundLane(
			WorldDirection worldDirection, LaneType[] laneTypes, string name = "",
			bool addTrafficLights = true, int distance = 100)
		{
			Lanes lanesCollection = _intersection.LanesCollection.Single(l => l.WorldDirection == worldDirection);

			InboundLane inboundLane = new(_intersection, lanesCollection, laneTypes, worldDirection, name, addTrafficLights, distance)
			{
				Distance = distance,
			};

			lanesCollection.InboundLanes!.Add(inboundLane);

			return this;
		}

		public IntersectionBuilder AddCarGenerator(ICarGenerator carGenerator, WorldDirection worldDirection, int laneNumber)
		{
			Lanes? lanes = _intersection.LanesCollection.Find(l => l.WorldDirection == worldDirection);

			if (lanes is null)
			{
				// TODO Handle
			}

			if (lanes!.InboundLanes is null || lanes!.InboundLanes!.Count < --laneNumber)
			{
				// TODO Handle
			}

			InboundLane inboundLane = lanes!.InboundLanes![laneNumber];

			inboundLane.CarGenerator = carGenerator;

			return this;
		}

		public ErrorOr<Intersection> Build()
		{
			// TODO: Check if intersection is not missing required elements
			return _intersection;
		}
	}
}
