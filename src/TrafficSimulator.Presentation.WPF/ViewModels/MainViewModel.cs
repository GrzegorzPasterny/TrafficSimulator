using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using System.Windows.Threading;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Presentation.WPF.Helpers;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly IntersectionSimulationHandlerFactory _intersectionSimulationHandlerFactory;
		private readonly ILogger<MainViewModel> _logger;
		private DispatcherTimer? _dispatcherTimer;
		private string _simulationMode = SimulationMode.RealTime;
		private ISimulationHandler _simulationHandler;
		private IntersectionElement _tempIntersectionElement = new();
		private IntersectionSimulation? _currentIntersectionSimulation;

		public IntersectionElementsOptions CanvasOptions { get; } = new();

		[ObservableProperty]
		private IntersectionElement _intersectionElement = new();

		[ObservableProperty]
		private int _simulationStepCounter = 0;

		[ObservableProperty]
		private double _stepsTaken;
		[ObservableProperty]
		private int _carsPassed;
		[ObservableProperty]
		private double _totalCarsIdleTimeMs;
		[ObservableProperty]
		private double _timeTakenSeconds;

		public ICommand LoadSimulationCommand { get; }
		public ICommand StartSimulationCommand { get; }

		public MainViewModel(IntersectionSimulationHandlerFactory intersectionSimulationHandlerFactory, ILogger<MainViewModel> logger)
		{
			_intersectionSimulationHandlerFactory = intersectionSimulationHandlerFactory;
			_simulationHandler = intersectionSimulationHandlerFactory.CreateHandler(_simulationMode);

			LoadSimulationCommand = new AsyncRelayCommand(LoadIntersection);
			StartSimulationCommand = new AsyncRelayCommand(RunSimulation);
			_logger = logger;
			_logger.LogInformation("MainViewModel initialized");

			LoadDummyIntersection();
		}

		private async Task LoadIntersection()
		{
			CleanUpTheSimulationData();

			string? jsonConfigurationFile = SimulationConfigurationFileLoader.LoadFromJsonFile();

			if (jsonConfigurationFile is null)
			{
				_logger.LogDebug("Attempt to select json configuration file was cancelled");
				return;
			}

			UnitResult<Error> loadIntersectionResult = await _simulationHandler.LoadIntersection(jsonConfigurationFile);

			if (loadIntersectionResult.IsFailure)
			{
				_logger.LogDebug("Simulation loading failed [Error = {Error}]", loadIntersectionResult.Error);
				// TODO: Print error to the user
				return;
			}

			Intersection intersection = _simulationHandler.IntersectionSimulation!.Intersection;
			_currentIntersectionSimulation = _simulationHandler.IntersectionSimulation;

			DrawIntersection(intersection);
		}

		private async Task RunSimulation()
		{
			CleanUpTheSimulationData();

			InitializeTimer(_simulationHandler.IntersectionSimulation.Options.StepTimespan);

			_dispatcherTimer!.Start();

			await _simulationHandler.Start();

			while (_simulationHandler.SimulationState.SimulationPhase
				is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished)
			{

				await Task.Delay(200);
			}

			_dispatcherTimer!.Stop();
			_dispatcherTimer = null;

			SimulationResults simulationResults = _simulationHandler.SimulationResults;
			StepsTaken = simulationResults.SimulationStepsTaken;
			CarsPassed = simulationResults.CarsPassed;
			TotalCarsIdleTimeMs = simulationResults.TotalCarsIdleTimeMs;
			TimeTakenSeconds = Math.Round(
				simulationResults.SimulationStepsTaken *
				_simulationHandler.IntersectionSimulation.Options.StepTimespan.TotalSeconds, 2);
		}

		private void CleanUpTheSimulationData()
		{
			StepsTaken = 0;
			CarsPassed = 0;
			TotalCarsIdleTimeMs = 0;
			TimeTakenSeconds = 0;
			SimulationStepCounter = 0;

			if (_currentIntersectionSimulation is not null &&
				_currentIntersectionSimulation.SimulationState.SimulationPhase != SimulationPhase.NotStarted)
			{
				_currentIntersectionSimulation.Reset();
			}
		}

		private void DrawIntersection(Intersection intersection)
		{
			_tempIntersectionElement = new IntersectionElement()
			{
				CarGeneratorsAreaOffset = CanvasOptions.CarGeneratorsAreaOffset
			};

			AddIntersectionCore(intersection);
			AddLanes(intersection, _tempIntersectionElement.IntersectionCoreElement);

			IntersectionElement = _tempIntersectionElement;
		}

		private void AddIntersectionCore(Intersection intersection)
		{
			int northLanes = GetAmountOfLanes(intersection, WorldDirection.North);
			int southLanes = GetAmountOfLanes(intersection, WorldDirection.South);
			int westLanes = GetAmountOfLanes(intersection, WorldDirection.West);
			int eastLanes = GetAmountOfLanes(intersection, WorldDirection.East);

			int maxVerticalLanesAmount = new[] { northLanes, southLanes, 1 }.Max();
			int maxHorizontalLanesAmount = new[] { westLanes, eastLanes, 1 }.Max();

			_tempIntersectionElement.IntersectionCoreElement =
				new IntersectionCoreElement()
				{
					Height = maxVerticalLanesAmount * CanvasOptions.LaneWidth,
					Width = maxHorizontalLanesAmount * CanvasOptions.LaneWidth,
				};
		}

		private void AddLanes(Intersection intersection, IntersectionCoreElement intersectionCoreElement)
		{
			AddNorthLanes(intersection, intersectionCoreElement);
			AddEastLanes(intersection, intersectionCoreElement);
			AddSouthLanes(intersection, intersectionCoreElement);
			AddWestLanes(intersection, intersectionCoreElement);
		}

		private void AddNorthLanes(Intersection intersection, IntersectionCoreElement intersectionCoreElement)
		{
			Lanes? northLanes = intersection.LanesCollection.FirstOrDefault(lanes => lanes.WorldDirection == WorldDirection.North);

			if (northLanes is null)
			{
				return;
			}

			int northLanesProcessed = 0;

			foreach (InboundLane lane in northLanes.InboundLanes)
			{
				double leftEdge = -intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge + CanvasOptions.LaneWidth * northLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = true,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.North,
					AnchorPointY = intersectionCoreElement.Height / 2,
					AnchorPointX = currentLanePositionFromTheLeft
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				northLanesProcessed++;
			}

			foreach (OutboundLane lane in northLanes.OutboundLanes)
			{
				double leftEdge = -intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge + CanvasOptions.LaneWidth * northLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = false,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.North,
					AnchorPointY = intersectionCoreElement.Height / 2,
					AnchorPointX = currentLanePositionFromTheLeft
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				northLanesProcessed++;
			}
		}

		private void AddEastLanes(Intersection intersection, IntersectionCoreElement intersectionCoreElement)
		{
			Lanes? eastLanes = intersection.LanesCollection.FirstOrDefault(lanes => lanes.WorldDirection == WorldDirection.East);

			if (eastLanes is null)
			{
				return;
			}

			int eastLanesProcessed = 0;

			foreach (InboundLane lane in eastLanes.InboundLanes)
			{
				double leftEdge = intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge - CanvasOptions.LaneWidth * eastLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = true,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.East,
					AnchorPointY = currentLanePositionFromTheLeft,
					AnchorPointX = intersectionCoreElement.Width / 2
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				eastLanesProcessed++;
			}

			foreach (OutboundLane lane in eastLanes.OutboundLanes)
			{
				double leftEdge = intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge - CanvasOptions.LaneWidth * eastLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = false,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.East,
					AnchorPointY = currentLanePositionFromTheLeft,
					AnchorPointX = intersectionCoreElement.Width / 2
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				eastLanesProcessed++;
			}
		}

		private void AddSouthLanes(Intersection intersection, IntersectionCoreElement intersectionCoreElement)
		{
			Lanes? southLanes = intersection.LanesCollection.FirstOrDefault(lanes => lanes.WorldDirection == WorldDirection.South);

			if (southLanes is null)
			{
				return;
			}

			int southLanesProcessed = 0;

			foreach (InboundLane lane in southLanes.InboundLanes)
			{
				double leftEdge = intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge - CanvasOptions.LaneWidth * southLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = true,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.South,
					AnchorPointY = -intersectionCoreElement.Width / 2,
					AnchorPointX = currentLanePositionFromTheLeft
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				southLanesProcessed++;
			}

			foreach (OutboundLane lane in southLanes.OutboundLanes)
			{
				double leftEdge = intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge - CanvasOptions.LaneWidth * southLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = false,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.South,
					AnchorPointY = -intersectionCoreElement.Width / 2,
					AnchorPointX = currentLanePositionFromTheLeft
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				southLanesProcessed++;
			}
		}

		private void AddWestLanes(Intersection intersection, IntersectionCoreElement intersectionCoreElement)
		{
			Lanes? westLanes = intersection.LanesCollection.FirstOrDefault(lanes => lanes.WorldDirection == WorldDirection.West);

			if (westLanes is null)
			{
				return;
			}

			int westLanesProcessed = 0;

			foreach (InboundLane lane in westLanes.InboundLanes)
			{
				double leftEdge = -intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge + CanvasOptions.LaneWidth * westLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = true,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.West,
					AnchorPointY = currentLanePositionFromTheLeft,
					AnchorPointX = -intersectionCoreElement.Width / 2
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				westLanesProcessed++;
			}

			foreach (OutboundLane lane in westLanes.OutboundLanes)
			{
				double leftEdge = -intersectionCoreElement.Width / 2;
				double currentLanePositionFromTheLeft = leftEdge + CanvasOptions.LaneWidth * westLanesProcessed;

				LaneElement laneElement = new LaneElement()
				{
					Inbound = false,
					ReferenceLaneId = lane.Id,
					Width = CanvasOptions.LaneWidth,
					WorldDirection = WorldDirection.West,
					AnchorPointY = currentLanePositionFromTheLeft,
					AnchorPointX = -intersectionCoreElement.Width / 2
				};

				_tempIntersectionElement.LaneElements.Add(laneElement);

				westLanesProcessed++;
			}
		}

		private int GetAmountOfLanes(Intersection intersection, WorldDirection worldDirection)
		{
			Lanes? lanes = intersection.LanesCollection.FirstOrDefault(lanes => lanes.WorldDirection == worldDirection);

			if (lanes == null)
			{
				return 0;
			}

			if (lanes.InboundLanes is null && lanes.OutboundLanes is null)
			{
				return 0;
			}

			if (lanes.InboundLanes is null)
			{
				return lanes.OutboundLanes!.Count;
			}

			if (lanes.OutboundLanes is null)
			{
				return lanes.InboundLanes!.Count;
			}

			return lanes.InboundLanes.Count + lanes.OutboundLanes.Count;
		}

		private void LoadDummyIntersection()
		{

		}

		private void InitializeTimer(TimeSpan refreshRate)
		{
			_dispatcherTimer = new DispatcherTimer
			{
				Interval = refreshRate
			};
			_dispatcherTimer.Tick += UpdateSimulationStatus;
		}

		private void UpdateSimulationStatus(object? sender, EventArgs e)
		{
			var status = _simulationHandler.SimulationState;

			SimulationStepCounter++;
		}
	}
}
