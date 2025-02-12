namespace TrafficSimulator.Domain.Models
{
	public class Intersection
	{
		public Lanes? NorthLanes { get; set; } = null;
		public Lanes? EastLanes { get; set; } = null;
		public Lanes? SouthLanes { get; set; } = null;
		public Lanes? WestLanes { get; set; } = null;

	}
}
