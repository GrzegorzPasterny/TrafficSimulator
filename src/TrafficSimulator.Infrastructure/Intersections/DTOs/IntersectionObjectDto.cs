using CSharpFunctionalExtensions;

namespace TrafficSimulator.Infrastructure.DTOs
{
	public class IntersectionObjectDto : Entity
	{
		public string Name { get; set; }
		public string ParentName { get; set; }
	}
}
