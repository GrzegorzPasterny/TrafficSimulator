using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models.IntersectionObjects
{
	public class TrafficLights : IntersectionObject
	{
		public TrafficLightState TrafficLightState { get; private set; }
		public bool IsOn { get; private set; } = true;
		public TrafficLightsOptions Options { get; internal set; } = new();

		public TrafficLights(Intersection root, IntersectionObject? parent, Action<TrafficLightsOptions>? options = null)
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

		internal override string BuildObjectName(string parentName)
		{
			return $"{parentName}.{nameof(TrafficLights)}";
		}
	}
}
