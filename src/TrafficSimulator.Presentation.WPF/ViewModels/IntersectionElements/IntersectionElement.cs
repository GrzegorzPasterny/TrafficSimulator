namespace TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements
{
	public class IntersectionElement
	{
		public IntersectionCoreElement IntersectionCoreElement { get; set; }
		public List<LaneElement> LaneElements { get; set; } = [];
		public List<TrafficLightsElement> TrafficLightsElements { get; set; } = [];
	}
}
