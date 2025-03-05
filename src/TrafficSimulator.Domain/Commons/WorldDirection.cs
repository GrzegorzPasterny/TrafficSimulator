using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Domain.Commons
{
	public enum WorldDirection
	{
		North,
		East,
		South,
		West,
	}

	public static class WorldDirectionExtensions
	{
		public static WorldDirection Rotate(this WorldDirection direction, LaneType laneType)
		{
			return laneType switch
			{
				LaneType.Straight => direction, // Keep moving in the same direction
				LaneType.Right => (WorldDirection)(((int)direction + 1) % 4), // 90° clockwise
				LaneType.Left => (WorldDirection)(((int)direction + 3) % 4), // 90° counterclockwise
				LaneType.TurnAround => (WorldDirection)(((int)direction + 2) % 4), // 180° turn
				_ => throw new ArgumentOutOfRangeException(nameof(laneType), "Invalid lane type")
			};
		}
	}
}
