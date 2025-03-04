using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements
{
	public class LaneElement
	{
		public int Width { get; set; }
		public int StartPointX { get; set; }
		public int StartPointY { get; set; }
		public WorldDirection WorldDirection { get; set; }
		public bool Inbound { get; set; }
	}
}
