namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public class IntersectionElementsOptions
	{
		public int CarWidth { get; set; } = 15;
		public int CarHeight => CarWidth * 2;
		public int LaneWidth => CarWidth * 2;
		public int CarGeneratorsAreaOffset { get; set; } = 100;

	}
}
