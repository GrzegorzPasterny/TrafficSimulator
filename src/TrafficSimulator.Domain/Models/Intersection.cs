namespace TrafficSimulator.Domain.Models
{
	public class Intersection
	{
		public List<Lanes> Lanes { get; set; } = new List<Lanes>();

		public IntersectionCore IntersectionCore { get; set; } = new();
	}
}
