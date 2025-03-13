using CSharpFunctionalExtensions;
using Serilog.Events;
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
using TrafficSimulator.Presentation.WPF.Helpers;
using TrafficSimulator.Presentation.WPF.ViewModels;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;

namespace TrafficSimulator.Presentation.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _mainViewModel;
	private readonly InMemorySink _inMemorySink;
	private Rectangle? _intersectionCore;
	private Dictionary<Guid, Rectangle> _inboundLanes = new();
	private Dictionary<Guid, Rectangle> _outboundLanes = new();
	private Dictionary<WorldDirection, Rectangle> _parkingLots = new();
	private Dictionary<WorldDirection, int> _numberOfCarsAtTheParkingLot = new();

	private Dictionary<Guid, Ellipse> _trafficLights = [];
	private Dictionary<Guid, Ellipse> _carLocations = [];
	private Thread _pollingThreadForLogging;

	public MainWindow(MainViewModel mainViewModel, InMemorySink inMemorySink)
	{
		InitializeComponent();
		DataContext = mainViewModel;
		_mainViewModel = mainViewModel;
		_inMemorySink = inMemorySink;
		_mainViewModel.PropertyChanged += DrawIntersection;
		_mainViewModel.TrafficLightUpdated += UpdateTrafficLights;
		_mainViewModel.CarLocationUpdated += UpdateCarLocation;
		_mainViewModel.NewSimulationStarted += ResetSimulation;

		// Set up an ongoing process to update the TextBox or DataGrid
		_pollingThreadForLogging = new Thread(PollQueueForLogs);
		_pollingThreadForLogging.IsBackground = true; // Make it a background thread so it terminates when the app closes
		_pollingThreadForLogging.Start();
	}

	private void PollQueueForLogs()
	{
		while (true)
		{
			if (_inMemorySink.Events.TryDequeue(out LogEvent logEvent))
			{
				var logEntry = new
				{
					Timestamp = logEvent.Timestamp.ToString("HH:mm:ss.fff"),
					Level = logEvent.Level.ToString(),
					Message = logEvent.RenderMessage()
				};

				if (LogDataGrid.Dispatcher.CheckAccess())
				{
					LogDataGrid.Items.Add(logEntry);
					LogDataGrid.ScrollIntoView(logEntry);
				}
				else
				{
					LogDataGrid.Dispatcher.Invoke(() =>
					{
						LogDataGrid.Items.Add(logEntry);
						LogDataGrid.ScrollIntoView(logEntry);
					});
				}
			}

			// Delay to avoid tight looping
			Thread.Sleep(100); // Sleep for 100ms (adjust based on requirements)
		}
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

			foreach (var numberOfCars in _numberOfCarsAtTheParkingLot)
			{
				_numberOfCarsAtTheParkingLot[numberOfCars.Key] = 0;
			}
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
					if (location.DistanceLeft == 0)
					{
						ParkCar(car, ((OutboundLane)location.Location).WorldDirection);
						return;
					}

					MoveCarThroughOutboundLane(car, location);
				}
			}
			else
			{
				Ellipse newCar = new()
				{
					Fill = BrushHelper.GetRandomNamedBrush(),
					Width = _mainViewModel.CanvasOptions.CarWidth,
					Height = _mainViewModel.CanvasOptions.CarWidth,
					Stroke = Brushes.Black,
					StrokeThickness = 1
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

	private void ParkCar(Ellipse car, WorldDirection worldDirection)
	{
		if (!_parkingLots.TryGetValue(worldDirection, out Rectangle parkingLot))
		{
			throw new ArgumentException();
		}

		if (!_numberOfCarsAtTheParkingLot.TryGetValue(worldDirection, out int numberOfCarsAtParkingLot))
		{
			throw new ArgumentException();
		}

		ParkCarAtParkingLot(car, parkingLot, numberOfCarsAtParkingLot);

		_numberOfCarsAtTheParkingLot[worldDirection]++;
	}

	private void ParkCarAtParkingLot(Ellipse car, Rectangle parkingLot, int numberOfCarsAtParkingLot)
	{
		// Get car size (assuming all cars are perfect circles)
		double carDiameter = car.Width; // Width == Height for circles

		// Get parking lot dimensions and position
		double lotX = Canvas.GetLeft(parkingLot);
		double lotY = Canvas.GetTop(parkingLot);
		double lotWidth = parkingLot.Width;
		double lotHeight = parkingLot.Height;

		// Calculate number of cars per row based on parking lot width
		int carsPerRow = (int)(lotWidth / carDiameter);
		if (carsPerRow == 0) return; // Prevent division by zero

		// Compute row and column for the new car
		int row = numberOfCarsAtParkingLot / carsPerRow;
		int col = numberOfCarsAtParkingLot % carsPerRow;

		// Calculate car position (centered within each grid cell)
		double carX = lotX + col * carDiameter;
		double carY = lotY + row * carDiameter;

		// Check if the car fits inside the parking lot
		if (carX + carDiameter <= lotX + lotWidth && carY + carDiameter <= lotY + lotHeight)
		{
			Canvas.SetLeft(car, carX);
			Canvas.SetTop(car, carY);
		}
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
		_parkingLots.Clear();
		_numberOfCarsAtTheParkingLot.Clear();

		DrawIntersectionCore(intersectionElement.IntersectionCoreElement);
		DrawLanesAndTrafficLights(intersectionElement.LaneElements, intersectionElement.CarGeneratorsAreaOffset);
		DrawParkingLots(intersectionElement.LaneElements, intersectionElement.CarGeneratorsAreaOffset);
	}

	private void DrawParkingLots(List<LaneElement> laneElements, int carGeneratorsAreaOffset)
	{
		IEnumerable<WorldDirection> outboundLanesWorldDirections = laneElements.Where(lane => !lane.IsInbound).Select(lane => lane.WorldDirection).Distinct();

		foreach (WorldDirection worldDirection in outboundLanesWorldDirections)
		{
			Rectangle parkingLot = new()
			{
				Fill = Brushes.LightGray,
				Stroke = Brushes.Black,
				StrokeThickness = 2,
				Height = carGeneratorsAreaOffset,
				Width = carGeneratorsAreaOffset,
			};

			double radius = SimulationCanvas.ActualHeight / 2 - carGeneratorsAreaOffset / 2;

			DrawerHelper.SetRectangleAtAngle(
				parkingLot,
				worldDirection.ToDegrees(),
				radius,
				SimulationCanvas.ActualWidth,
				SimulationCanvas.ActualHeight);

			_parkingLots.Add(worldDirection, parkingLot);
			_numberOfCarsAtTheParkingLot.Add(worldDirection, 0);

			SimulationCanvas.Children.Add(parkingLot);
		}

		//DrawNorthParkingLot(carGeneratorsAreaOffset);
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
			SimulationCanvas.Children.Add(laneRectangle);

			switch (laneElement.WorldDirection)
			{
				case WorldDirection.North:
					laneRectangle.Width = laneElement.Width;
					laneRectangle.Height = SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - carGeneratorsAreaOffset;
					laneRectangle.Fill = Brushes.Black;
					laneRectangle.Stroke = Brushes.White;

					if (laneElement.IsInbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, carGeneratorsAreaOffset);
					Canvas.SetLeft(laneRectangle, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX);

					if (laneElement.IsInbound)
						AddTrafficLights(laneElement);

					if (laneElement.IsInbound)
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

					if (laneElement.IsInbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2);

					if (laneElement.IsInbound)
						AddTrafficLights(laneElement);

					if (laneElement.IsInbound)
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

					if (laneElement.IsInbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY);
					Canvas.SetLeft(laneRectangle, laneElement.AnchorPointX + SimulationCanvas.ActualWidth / 2 - laneElement.Width);

					if (laneElement.IsInbound)
						AddTrafficLights(laneElement);

					if (laneElement.IsInbound)
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

					if (laneElement.IsInbound)
						laneRectangle.Fill = Brushes.Chocolate;

					Canvas.SetTop(laneRectangle, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width);
					Canvas.SetLeft(laneRectangle, carGeneratorsAreaOffset);

					if (laneElement.IsInbound)
						AddTrafficLights(laneElement);

					if (laneElement.IsInbound)
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

		}
	}

	private void AddTrafficLights(LaneElement laneElement)
	{
		Ellipse ellipse = new Ellipse()
		{
			Height = laneElement.Width / 1.5,
			Width = laneElement.Width / 1.5,
			Fill = Brushes.Red,
		};

		switch (laneElement.WorldDirection)
		{
			case WorldDirection.North:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX + laneElement.Width / 2 - ellipse.Width / 2);
				break;
			case WorldDirection.East:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY + laneElement.Width / 2 - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - ellipse.Width / 2);
				break;
			case WorldDirection.South:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - laneElement.Width / 2 - ellipse.Width / 2);
				break;
			case WorldDirection.West:
				Canvas.SetTop(ellipse, SimulationCanvas.ActualHeight / 2 - laneElement.AnchorPointY - laneElement.Width / 2 - ellipse.Height / 2);
				Canvas.SetLeft(ellipse, SimulationCanvas.ActualWidth / 2 + laneElement.AnchorPointX - ellipse.Width / 2);
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