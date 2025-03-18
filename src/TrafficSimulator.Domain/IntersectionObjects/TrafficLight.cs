using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Lights;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	// TODO: Make it an interface
	// TODO: Add NoTrafficLights implementation for intersections without Traffic Lights
	public class TrafficLight : IntersectionObject, IEquatable<TrafficLight>
	{
		public TrafficLightState TrafficLightState { get; private set; }
		public bool IsOn { get; private set; } = true;
		public TrafficLightsOptions Options { get; internal set; } = new();

		public TrafficLight(Intersection root, IntersectionObject? parent, Action<TrafficLightsOptions>? options = null)
			: base(root, parent)
		{
			options?.Invoke(Options);
			TrafficLightState = Options.InitialState;
		}

		public UnitResult<Error> SwitchToGreen()
		{
			if (!IsOn)
			{
				return DomainErrors.TrafficLightsOff(Id);
			}

			if (TrafficLightState == TrafficLightState.Green)
			{
				return DomainErrors.TrafficLightsAlreadyInRequestedState(Id, TrafficLightState);
			}

			//TODO: Turn to Orange first

			TrafficLightState = TrafficLightState.Green;
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> SwitchToRed()
		{
			if (!IsOn)
			{
				return DomainErrors.TrafficLightsOff(Id);
			}

			if (TrafficLightState == TrafficLightState.Red)
			{
				return DomainErrors.TrafficLightsAlreadyInRequestedState(Id, TrafficLightState);
			}

			//TODO: Turn to Orange first

			TrafficLightState = TrafficLightState.Red;
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> TurnOff()
		{
			if (!IsOn)
			{
				return DomainErrors.TrafficLightsOverallState(Id, IsOn);
			}

			IsOn = false;
			TrafficLightState = TrafficLightState.Off;
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> TurnOn()
		{
			if (IsOn)
			{
				return DomainErrors.TrafficLightsOverallState(Id, IsOn);
			}

			IsOn = true;
			TrafficLightState = Options.InitialState;
			return UnitResult.Success<Error>();
		}

		public override bool Equals(object? obj)
		{
			return obj is TrafficLight other && Equals(other);
		}

		public bool Equals(TrafficLight? other)
		{
			if (other == null) return false;

			return base.Equals(other);
		}

		public override string ToString()
		{
			return $"[Name = {Name}, " +
				$"State = {TrafficLightState}]";
		}
	}
}
