using System.Windows.Controls;
using System.Windows.Shapes;

namespace TrafficSimulator.Presentation.WPF.Helpers
{
	public static class DrawerHelper
	{
		/// <summary>
		/// Creates a UI element (rectangle) positioned at a specific angle and distance from the center of a canvas.
		/// </summary>
		/// <param name="rectangle">The element to be manipulated</param>
		/// <param name="angle">The angle (in degrees) where the element should be placed. 0° is right, 90° is up.</param>
		/// <param name="radius">The distance from the center of the canvas.</param>
		/// <param name="canvasWidth">The width of the canvas where the element will be placed.</param>
		/// <param name="canvasHeight">The height of the canvas where the element will be placed.</param>
		public static void SetRectangleAtAngle(this Rectangle rectangle, double angle, double radius, double canvasWidth, double canvasHeight)
		{
			// Convert angle from degrees to radians
			double radians = angle * Math.PI / 180;

			// Calculate center of the canvas
			double centerX = canvasWidth / 2;
			double centerY = canvasHeight / 2;

			// Compute correct position, compensating for element dimensions
			double x = centerX + radius * Math.Cos(radians) - rectangle.Width / 2;
			double y = centerY + radius * Math.Sin(radians) - rectangle.Height / 2;

			// Apply position
			Canvas.SetLeft(rectangle, x);
			Canvas.SetTop(rectangle, y);
		}
	}

}
