using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Presentation.WPF.ViewModels;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;

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
		_mainViewModel.PropertyChanged += ViewModel_PropertyChanged;
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(MainViewModel.IntersectionElement))
		{
			DrawIntersection(_mainViewModel.IntersectionElement);
		}
	}

	private void DrawIntersection(IntersectionElement intersectionElement)
	{
		SimulationCanvas.Children.Clear();

		DrawIntersectionCore(intersectionElement.IntersectionCoreElement);
		DrawLanes(intersectionElement.LaneElements);
	}

	private void DrawIntersectionCore(IntersectionCoreElement intersectionCoreElement)
	{
		if (intersectionCoreElement is null)
		{
			return;
		}

		Rectangle intersectionCore = new Rectangle()
		{
			Height = intersectionCoreElement.Height,
			Width = intersectionCoreElement.Width,
			Fill = Brushes.Gray,
			Stroke = Brushes.Black,
			StrokeThickness = 2
		};

		Canvas.SetTop(intersectionCore, SimulationCanvas.ActualHeight / 2 - intersectionCoreElement.Height / 2);
		Canvas.SetLeft(intersectionCore, SimulationCanvas.ActualWidth / 2 - intersectionCoreElement.Width / 2);

		SimulationCanvas.Children.Add(intersectionCore);
	}

	private void DrawLanes(List<LaneElement> laneElements)
	{
		foreach (var laneElement in laneElements)
		{
			Rectangle laneRectangle = new();

			switch (laneElement.WorldDirection)
			{
				case WorldDirection.North:
					laneRectangle.Width = laneElement.Width;
					laneRectangle.Height = SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					if (laneElement.Inbound)
						AddTrafficLights(laneElement.AnchorPointX, laneElement.AnchorPointY, laneElement.Width, WorldDirection.North);

					Canvas.SetTop(laneRectangle, 0);
					Canvas.SetLeft(laneRectangle, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX);
					break;
				case WorldDirection.East:
					laneRectangle.Width = SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointX;
					laneRectangle.Height = laneElement.Width;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2);
					break;
				case WorldDirection.South:
					laneRectangle.Width = laneElement.Width;
					laneRectangle.Height = SimulationCanvas.ActualHeight / 2 + laneElement.AnchorPointY;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2 - laneElement.Width);
					break;
				case WorldDirection.West:
					laneRectangle.Width = SimulationCanvas.ActualHeight / 2 + laneElement.AnchorPointX;
					laneRectangle.Height = laneElement.Width;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 + laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, 0);
					break;
				default:
					break;
			}

			SimulationCanvas.Children.Add(laneRectangle);
		}
	}

	private void AddTrafficLights(double anchorPointX, double anchorPointY, int laneWidth, WorldDirection worldDirection)
	{
		Ellipse ellipse = new Ellipse()
		{
			Height = laneWidth / 2,
			Width = laneWidth / 2,
			Fill = Brushes.Red,
		};

		switch (worldDirection)
		{
			case WorldDirection.North:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - anchorPointY - laneWidth / 2 + ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + anchorPointX + laneWidth / 2 - ellipse.Width / 2);
				break;
			case WorldDirection.West:
				break;
			case WorldDirection.South:
				break;
			case WorldDirection.East:
				break;
			default:
				break;
		}

		// Bring to front
		Panel.SetZIndex(ellipse, 100);

		SimulationCanvas.Children.Add(ellipse);
	}
}