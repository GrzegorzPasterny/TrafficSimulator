using TrafficSimulator.Domain.Commons;

namespace TrafficSimulator.Presentation.WPF.Extensions
{
	public static class WorldDirectionExtensions
	{
		public static double ToDegrees(this WorldDirection worldDirection)
		{
			switch (worldDirection)
			{
				case WorldDirection.North:
					return 270;
				case WorldDirection.East:
					return 0;
				case WorldDirection.South:
					return 90;
				case WorldDirection.West:
					return 180;
				default:
					throw new ArgumentException();
			}
		}
	}
}
