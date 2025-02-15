namespace TrafficSimulator.Domain.Models.Intersection
{
    public class TrafficLightsOptions
    {
        public TrafficLightState InitialState { get; set; } = TrafficLightState.Red;
        public TimeSpan SwitchLightTimespanMs { get; set; } = TimeSpan.FromSeconds(2);
    }
}
