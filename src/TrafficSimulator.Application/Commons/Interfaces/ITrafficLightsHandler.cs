namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ITrafficLightsHandler
	{
		Task SetLights(TimeSpan timeElapsed);
	}
}
