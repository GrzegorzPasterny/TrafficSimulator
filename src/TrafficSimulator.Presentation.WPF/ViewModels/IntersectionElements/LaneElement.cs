using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements
{
	public class LaneElement
	{
		public int Width { get; set; }
		public double AnchorPointX { get; set; }
		public double AnchorPointY { get; set; }
		public WorldDirection WorldDirection { get; set; }
		public bool IsInbound { get; set; }
		public bool ContainsCarGenerator { get; set; }
		public Guid ReferenceLaneId { get; internal set; }
		public Guid? TrafficLightsId { get; internal set; }
	}
}
