using System.Reflection;
using System.Windows.Media;

namespace TrafficSimulator.Presentation.WPF.Helpers
{
	public static class BrushHelper
	{
		private static readonly Random _random = new Random();
		private static readonly Brush[] _namedBrushes = typeof(Colors)
			.GetProperties(BindingFlags.Static | BindingFlags.Public)
			.Select(p => new SolidColorBrush((Color)p.GetValue(null)!))
			.ToArray();

		public static Brush GetRandomNamedBrush()
		{
			return _namedBrushes[_random.Next(_namedBrushes.Length)];
		}

		public static Brush GetRandomBrush()
		{
			return new SolidColorBrush(Color.FromRgb(
				(byte)_random.Next(256),
				(byte)_random.Next(256),
				(byte)_random.Next(256)
			));
		}
	}
}
