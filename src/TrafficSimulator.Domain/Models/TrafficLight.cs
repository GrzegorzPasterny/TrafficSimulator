using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Domain.Models
{
	public class TrafficLight : Entity
	{
		public TrafficLightState TrafficLightState { private get; set; } = TrafficLightState.Red;
		public bool IsOn { private get; set; } = true;

		public TrafficLight()
		{

		}

		public UnitResult<Error> SwitchToGreen()
		{
			if (!IsOn)
			{
				return Errors.TrafficLightsOff(Id);
			}

			if (TrafficLightState != TrafficLightState.Green)
			{
				return Errors.TrafficLightsAlreadyInRequestedState(Id, TrafficLightState);
			}

			//TODO: Turn to Orange first

			TrafficLightState = TrafficLightState.Green;
			return UnitResult.Success<Error>();
		}

		public Result SwitchToRed()
		{

		}

		public Result TurnOff()
		{

		}
	}
}
