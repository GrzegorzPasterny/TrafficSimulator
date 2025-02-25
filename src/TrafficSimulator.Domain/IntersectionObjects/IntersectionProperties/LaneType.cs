
namespace TrafficSimulator.Domain.Models
{
	public enum LaneType
	{
		Straight,
		Right,
		Left,
		TurnAround
	}

	public static class LaneTypeHelper
	{
		public static LaneType[] Straight() => [LaneType.Straight];
		public static LaneType[] Left() => [LaneType.Left];
		public static LaneType[] Right() => [LaneType.Right];
		public static LaneType[] StraightAndRight() => [LaneType.Straight, LaneType.Right];
		public static LaneType[] StraightAndLeft() => [LaneType.Straight, LaneType.Left];
		public static LaneType[] LeftAndRight() => [LaneType.Left, LaneType.Right];
		public static LaneType[] StraightLeftAndRight() => [LaneType.Straight, LaneType.Left, LaneType.Right];
	}
}
