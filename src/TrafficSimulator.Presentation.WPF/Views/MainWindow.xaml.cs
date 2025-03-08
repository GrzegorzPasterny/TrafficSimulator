using CSharpFunctionalExtensions;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
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

	private Rectangle? _intersectionCore;
	private Dictionary<Guid, Rectangle> _inboundLanes = new();
	private Dictionary<Guid, Rectangle> _outboundLanes = new();

	private Dictionary<Guid, Ellipse> _trafficLights = [];
	private Dictionary<Guid, Ellipse> _carLocations = [];

	public MainWindow(MainViewModel mainViewModel)
	{
		InitializeComponent();
		DataContext = mainViewModel;
		_mainViewModel = mainViewModel;
		_mainViewModel.PropertyChanged += DrawIntersection;
		_mainViewModel.TrafficLightUpdated += UpdateTrafficLights;
		_mainViewModel.CarLocationUpdated += UpdateCarLocation;
		_mainViewModel.NewSimulationStarted += ResetSimulation;
	}

	private void ResetSimulation()
	{
		Dispatcher.BeginInvoke(() =>
		{
			foreach (var car in _carLocations.Select(c => c.Value))
			{
				SimulationCanvas.Children.Remove(car);
			}

			_carLocations.Clear();
		});
	}

	private void UpdateCarLocation(Guid guid, CarLocation location)
	{
		Dispatcher.BeginInvoke(() =>
		{
			if (_carLocations.TryGetValue(guid, out Ellipse car))
			{
				if (location.Location is IntersectionCore)
				{
					MoveCarThroughIntersectionCore(car, location);
				}
				else if (location.Location is InboundLane)
				{
					MoveCarThroughInboundLane(car, location);
				}
				else if (location.Location is OutboundLane)
				{
					MoveCarThroughOutboundLane(car, location);
				}
			}
			else
			{
				Ellipse newCar = new()
				{
					Fill = Brushes.Blue,
					Width = _mainViewModel.CanvasOptions.CarWidth,
					Height = _mainViewModel.CanvasOptions.CarWidth,
				};

				_carLocations.Add(guid, newCar);
				SimulationCanvas.Children.Add(newCar);

				Panel.SetZIndex(newCar, 200);

				if (location.Location is IntersectionCore)
				{
					MoveCarThroughIntersectionCore(newCar, location);
				}
				else if (location.Location is InboundLane)
				{
					MoveCarThroughInboundLane(newCar, location);
				}
				else if (location.Location is OutboundLane)
				{
					MoveCarThroughOutboundLane(newCar, location);
				}
			}
		});
	}

	private void MoveCarThroughInboundLane(Ellipse newCar, CarLocation location)
	{
		InboundLane inboundLane = (InboundLane)location.Location;

		Rectangle laneOfTheCar = _inboundLanes[inboundLane.Id];

		double upperEdge = Canvas.GetTop(laneOfTheCar);
		double lowerEdge = upperEdge + laneOfTheCar.Height;
		double leftEdge = Canvas.GetLeft(laneOfTheCar);
		double rightEdge = leftEdge + laneOfTheCar.Width;

		switch (inboundLane.WorldDirection)
		{
			case WorldDirection.North:
				{
					double carCenterOfLaneWidth = (leftEdge + rightEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualHeight * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = upperEdge + carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.East:
				{
					double carCenterOfLaneWidth = (upperEdge + lowerEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualWidth * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = rightEdge - carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.South:
				{
					double carCenterOfLaneWidth = (leftEdge + rightEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualHeight * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = lowerEdge - carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.West:
				{
					double carCenterOfLaneWidth = (upperEdge + lowerEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualWidth * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = leftEdge + carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carPositionOnTheLane);
				}
				break;
			default:
				break;
		}
	}

	private void MoveCarThroughOutboundLane(Ellipse newCar, CarLocation location)
	{
		OutboundLane outboundLane = (OutboundLane)location.Location;

		Rectangle laneOfTheCar = _outboundLanes[outboundLane.Id];

		double upperEdge = Canvas.GetTop(laneOfTheCar);
		double lowerEdge = upperEdge + laneOfTheCar.Height;
		double leftEdge = Canvas.GetLeft(laneOfTheCar);
		double rightEdge = leftEdge + laneOfTheCar.Width;

		switch (outboundLane.WorldDirection)
		{
			case WorldDirection.North:
				{
					double carCenterOfLaneWidth = (leftEdge + rightEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualHeight * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = lowerEdge - carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.East:
				{
					double carCenterOfLaneWidth = (upperEdge + lowerEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualWidth * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = leftEdge + carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.South:
				{
					double carCenterOfLaneWidth = (leftEdge + rightEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualHeight * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = upperEdge + carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carPositionOnTheLane);
				}
				break;
			case WorldDirection.West:
				{
					double carCenterOfLaneWidth = (upperEdge + lowerEdge) / 2 - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetTop(newCar, carCenterOfLaneWidth);

					double carProgressOnTheLane = laneOfTheCar.ActualWidth * location.CurrentDistance / location.Location.Distance;
					double carPositionOnTheLane = rightEdge - carProgressOnTheLane - _mainViewModel.CanvasOptions.CarWidth / 2;
					Canvas.SetLeft(newCar, carPositionOnTheLane);
				}
				break;
			default:
				break;
		}

	}

	private void MoveCarThroughIntersectionCore(Ellipse newCar, CarLocation location)
	{
		// TODO: Use location and make Car turn
		Canvas.SetLeft(newCar, SimulationCanvas.ActualWidth / 2 - _mainViewModel.CanvasOptions.CarWidth / 2);
		Canvas.SetTop(newCar, SimulationCanvas.ActualHeight / 2 - _mainViewModel.CanvasOptions.CarWidth / 2);
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

	private void DrawIntersection(object? sender, PropertyChangedEventArgs e)
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
		_intersectionCore = null;
		_inboundLanes.Clear();
		_outboundLanes.Clear();

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
		_intersectionCore = intersectionCore;
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

					if (laneElement.Inbound)
					{
						_inboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
					else
					{
						_outboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
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

					if (laneElement.Inbound)
					{
						_inboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
					else
					{
						_outboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
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

					if (laneElement.Inbound)
					{
						_inboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
					else
					{
						_outboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
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

					if (laneElement.Inbound)
					{
						_inboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
					else
					{
						_outboundLanes.Add(laneElement.ReferenceLaneId, laneRectangle);
					}
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