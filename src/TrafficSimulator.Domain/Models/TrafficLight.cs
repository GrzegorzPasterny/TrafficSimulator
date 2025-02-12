using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class TrafficLight : Entity
	{
		public TrafficLightState TrafficLightState { private get; set; }
		public bool IsOn { private get; set; } = true;
		public TrafficLightsOptions Options { get; internal set; } = new();

		public TrafficLight(Action<TrafficLightsOptions>? options = null)
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
			if (IsOn)
			{
				return DomainErrors.TrafficLightsOff(Id);
			}

			IsOn = false;
			return UnitResult.Success<Error>();
		}

		public UnitResult<Error> TurnOn()
		{
			if (!IsOn)
			{
				return DomainErrors.TrafficLightsOff(Id);
			}

			IsOn = true;
			TrafficLightState = Options.InitialState;
			return UnitResult.Success<Error>();
		}
	}
}
