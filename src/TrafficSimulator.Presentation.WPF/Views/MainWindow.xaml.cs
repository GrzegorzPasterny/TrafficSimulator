using CSharpFunctionalExtensions;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Presentation.WPF.Extensions;
using TrafficSimulator.Presentation.WPF.ViewModels;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;

namespace TrafficSimulator.Presentation.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _mainViewModel;
	private Dictionary<Guid, Ellipse> _trafficLights = [];

	public MainWindow(MainViewModel mainViewModel)
	{
		InitializeComponent();
		DataContext = mainViewModel;
		_mainViewModel = mainViewModel;
		_mainViewModel.PropertyChanged += ViewModel_PropertyChanged;
		_mainViewModel.TrafficLightUpdated += UpdateTrafficLights;
	}

	private void UpdateTrafficLights(Guid guid, TrafficLightState state)
	{
		Maybe<Ellipse> ellipse = _trafficLights.TryFind(guid);

		if (ellipse.HasNoValue)
		{
			// TODO: Handle
			throw new ArgumentOutOfRangeException();
		}

		Dispatcher.BeginInvoke(() =>
		{
			ellipse.Value.Fill = state.ToColor();
		});
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
		_trafficLights.Clear();

		DrawIntersectionCore(intersectionElement.IntersectionCoreElement);
		DrawLanesAndTrafficLights(intersectionElement.LaneElements, intersectionElement.CarGeneratorsAreaOffset);
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

	private void DrawLanesAndTrafficLights(List<LaneElement> laneElements, int carGeneratorsAreaOffset)
	{
		foreach (var laneElement in laneElements)
		{
			Rectangle laneRectangle = new();

			switch (laneElement.WorldDirection)
			{
				case WorldDirection.North:
					laneRectangle.Width = laneElement.Width;
					laneRectangle.Height = SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - carGeneratorsAreaOffset;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					if (laneElement.Inbound)
						AddTrafficLights(laneElement!);

					Canvas.SetTop(laneRectangle, carGeneratorsAreaOffset);
					Canvas.SetLeft(laneRectangle, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX);
					break;
				case WorldDirection.East:
					laneRectangle.Width = SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointX - carGeneratorsAreaOffset;
					laneRectangle.Height = laneElement.Width;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					if (laneElement.Inbound)
						AddTrafficLights(laneElement);

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2);
					break;
				case WorldDirection.South:
					laneRectangle.Width = laneElement.Width;
					laneRectangle.Height = SimulationCanvas.ActualHeight / 2 + laneElement.AnchorPointY - carGeneratorsAreaOffset;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					if (laneElement.Inbound)
						AddTrafficLights(laneElement);

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2 - laneElement.Width);
					break;
				case WorldDirection.West:
					laneRectangle.Width = SimulationCanvas.ActualHeight / 2 + laneElement.AnchorPointX - carGeneratorsAreaOffset;
					laneRectangle.Height = laneElement.Width;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.Inbound)
						laneRectangle.Fill = Brushes.Chocolate;

					if (laneElement.Inbound)
						AddTrafficLights(laneElement);

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width);
					Canvas.SetLeft(laneRectangle, carGeneratorsAreaOffset);
					break;
				default:
					break;
			}

			SimulationCanvas.Children.Add(laneRectangle);
		}
	}

	private void AddTrafficLights(LaneElement laneElement)
	{
		Ellipse ellipse = new Ellipse()
		{
			Height = laneElement.Width / 2,
			Width = laneElement.Width / 2,
			Fill = Brushes.Red,
		};

		switch (laneElement.WorldDirection)
		{
			case WorldDirection.North:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width / 2 + ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX + laneElement.Width / 2 - ellipse.Width / 2);
				break;
			case WorldDirection.East:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY + laneElement.Width / 2 - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - laneElement.Width / 2 + ellipse.Width / 2);
				break;
			case WorldDirection.South:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width / 2 + ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - laneElement.Width / 2 - ellipse.Width / 2);
				break;
			case WorldDirection.West:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width / 2 - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - laneElement.Width / 2 + ellipse.Width / 2);
				break;
			default:
				break;
		}

		// Bring to front
		Panel.SetZIndex(ellipse, 100);

		SimulationCanvas.Children.Add(ellipse);

		if (laneElement.TrafficLightsId is not null)
		{
			_trafficLights.Add((Guid)laneElement.TrafficLightsId!, ellipse);
		}
	}
}