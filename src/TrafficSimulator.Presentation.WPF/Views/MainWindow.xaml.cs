using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Presentation.WPF.ViewModels;

namespace TrafficSimulator.Presentation.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _mainViewModel;

	public MainWindow(MainViewModel mainViewModel)
	{
		InitializeComponent();
		DataContext = mainViewModel;
		_mainViewModel = mainViewModel;

		//DrawDummyIntersection();
		DrawIntersection();
	}

	private void DrawIntersection()
	{
		SimulationCanvas.Children.Clear();

		foreach (var element in _mainViewModel.TrafficElements)
		{
			UIElement uiElement = null;

			switch (element.Type)
			{
				case TrafficElementType.IntersectionCore:
					uiElement = new Rectangle
					{
						Width = element.Width,
						Height = element.Height,
						Fill = (Brush)new BrushConverter().ConvertFromString(element.Color)
					};
					break;

				case TrafficElementType.Lane:
					uiElement = new Rectangle
					{
						Width = element.Width,
						Height = element.Height,
						Fill = (Brush)new BrushConverter().ConvertFromString(element.Color)
					};
					break;

				case TrafficElementType.TrafficLight:
					uiElement = new Ellipse
					{
						Width = element.Width,
						Height = element.Height,
						Fill = (Brush)new BrushConverter().ConvertFromString(element.Color)
					};
					break;
			}

			if (uiElement != null)
			{
				Canvas.SetLeft(uiElement, element.X);
				Canvas.SetTop(uiElement, element.Y);
				SimulationCanvas.Children.Add(uiElement);
			}
		}
	}

	private void DrawDummyIntersection()
	{
		SimulationCanvas.Children.Clear();

		// Draw Intersection Core (Gray Square)
		var intersectionCore = new Rectangle
		{
			Width = 100,
			Height = 100,
			Fill = Brushes.DarkGray
		};
		Canvas.SetLeft(intersectionCore, 250);
		Canvas.SetTop(intersectionCore, 250);
		SimulationCanvas.Children.Add(intersectionCore);

		// Draw Inbound Lanes
		DrawLane(300, 0, 300, 250, Brushes.Black); // North
		DrawLane(300, 600, 300, 350, Brushes.Black); // South
		DrawLane(600, 300, 350, 300, Brushes.Black); // East
		DrawLane(0, 300, 250, 300, Brushes.Black); // West

		// Draw Outbound Lanes
		DrawLane(300, 100, 300, 250, Brushes.Gray); // North
		DrawLane(300, 500, 300, 350, Brushes.Gray); // South
		DrawLane(500, 300, 350, 300, Brushes.Gray); // East
		DrawLane(100, 300, 250, 300, Brushes.Gray); // West

		// Draw Traffic Lights
		DrawTrafficLight(290, 230); // North
		DrawTrafficLight(290, 370); // South
		DrawTrafficLight(370, 290); // East
		DrawTrafficLight(230, 290); // West
	}

	private void DrawLane(double x1, double y1, double x2, double y2, Brush color)
	{
		var laneLine = new Line
		{
			Stroke = color,
			StrokeThickness = 6,
			X1 = x1,
			Y1 = y1,
			X2 = x2,
			Y2 = y2
		};
		SimulationCanvas.Children.Add(laneLine);
	}

	private void DrawTrafficLight(double left, double top)
	{
		var trafficLight = new Ellipse
		{
			Width = 20,
			Height = 20,
			Fill = Brushes.Red
		};
		Canvas.SetLeft(trafficLight, left);
		Canvas.SetTop(trafficLight, top);
		SimulationCanvas.Children.Add(trafficLight);
	}
}