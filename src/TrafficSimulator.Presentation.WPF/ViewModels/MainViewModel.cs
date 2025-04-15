using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Application.Simulation;
using TrafficSimulator.Application.TrafficLights.Handlers.Factory;
using TrafficSimulator.Domain.Cars;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Domain.Models.Lights;
using TrafficSimulator.Domain.Simulation;
using TrafficSimulator.Presentation.WPF.Extensions;
using TrafficSimulator.Presentation.WPF.Helpers;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;
using TrafficSimulator.Presentation.WPF.ViewModels.Items;
using TrafficSimulator.Presentation.WPF.ViewModels.SimulationElements;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly IntersectionSimulationHandlerFactory _intersectionSimulationHandlerFactory;
		private readonly ILogger<MainViewModel> _logger;
		private ISimulationHandler _simulationHandler;
		private IntersectionElement _tempIntersectionElement = new();
		private IntersectionSimulation? _currentIntersectionSimulation;

		public IntersectionElementsOptions CanvasOptions { get; } = new();

		[ObservableProperty]
		private IntersectionElement _intersectionElement = new();

		public event Action<Guid, TrafficLightState>? TrafficLightUpdated;
		public event Action<Guid, CarLocation>? CarLocationUpdated;
		public event Action? NewSimulationStarted;

		private readonly DispatcherTimer _simulationTimer;
		private DateTime _simulationStartTime;

		[ObservableProperty]
		private string _simulationModeName;

		[ObservableProperty]
		private int _simulationStepCounter = 0;
		[ObservableProperty]
		private string _elapsedSimulationTime = "00:00:000";

		[ObservableProperty]
		private double _stepsTaken;
		[ObservableProperty]
		private int _carsPassed;
		[ObservableProperty]
		private double _totalCarsIdleTimeMs;
		[ObservableProperty]
		private double _calculationTimeSeconds;
		[ObservableProperty]
		private TrafficPhaseItem _currentTrafficPhaseItem;
		[ObservableProperty]
		private ObservableCollection<TrafficPhaseItem> _trafficPhaseItems = new ObservableCollection<TrafficPhaseItem>();
		[ObservableProperty]
		private TrafficLightsModeItem _currentTrafficLightsModeItem;
		[ObservableProperty]
		private ObservableCollection<TrafficLightsModeItem> _trafficLightsModeItems = new ObservableCollection<TrafficLightsModeItem>(
			TrafficLightHandlerTypes.Modes.Select(m => m.ToTrafficPhaseModeItem()));

		[ObservableProperty]
		private string _simulationName;
		[ObservableProperty]
		private string _simulationId;
		[ObservableProperty]
		private string _simulationFilePath;
		[ObservableProperty]
		private int _simulationTimespanMs;
		[ObservableProperty]
		private int _minimalDistanceBetweenCars;

		public ICommand LoadSimulationCommand { get; }
		public ICommand StartSimulationCommand { get; }
		public ICommand ChangeSimulationModeCommand { get; }
		public ICommand AbortSimulationCommand { get; }

		public MainViewModel(IntersectionSimulationHandlerFactory intersectionSimulationHandlerFactory, IOptions<SimulationOptions> options, ILogger<MainViewModel> logger)
		{
			_logger = logger;

			SetInitialSimulationOptions(options.Value);

			_intersectionSimulationHandlerFactory = intersectionSimulationHandlerFactory;
			CreateAndConfigureSimulationHandler();

			LoadSimulationCommand = new AsyncRelayCommand(LoadIntersection);
			StartSimulationCommand = new AsyncRelayCommand(RunSimulation);
			ChangeSimulationModeCommand = new RelayCommand(ChangeSimulationMode);
			AbortSimulationCommand = new AsyncRelayCommand(AbortSimmulation);

			// TODO: Implement
			LoadDummyIntersection();

			_simulationTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(40)
			};
			_simulationTimer.Tick += SimulationTimerTick;

			_logger.LogInformation("MainViewModel initialized");
		}

		partial void OnCurrentTrafficPhaseItemChanged(TrafficPhaseItem value)
		{
			ChangeTrafficPhase(value);
		}

		private void ChangeTrafficPhase(TrafficPhaseItem newTrafficPhase)
		{
			if (_currentIntersectionSimulation is null)
				return;

			if (SimulationModeName == SimulationMode.InMemory)
				return;

			if (!_currentIntersectionSimulation.SimulationState.IsInProgress)
				return;

			// TODO: This method is triggered not only after user manual change of the Traffic Phase in GUI,
			// but also when simulation engine change the phase. In latter scenatio this call is redundant.
			UnitResult<Error> trafficPhaseChangeResult =
				_simulationHandler.ChangeTrafficPhase(newTrafficPhase.Name);

			if (trafficPhaseChangeResult.IsFailure)
			{
				_logger.LogError("Traffic Lights phase manual change failed [Error = {Error}]",
					trafficPhaseChangeResult.Error);
			}
		}

		partial void OnCurrentTrafficLightsModeItemChanged(TrafficLightsModeItem value)
		{
			ChangeTrafficLightsMode(value);
		}

		private void ChangeTrafficLightsMode(TrafficLightsModeItem value)
		{
			if (_simulationHandler is null || _simulationHandler.SimulationState is null)
			{
				return;
			}

			if (_simulationHandler.SimulationState.IsInProgress)
			{
				// TODO: Consider allowing to change the traffic lights mode in ongoing simulation
				return;
			}

			_currentIntersectionSimulation!.Options.TrafficLightHandlerType = value.Name;
			ReloadCurrentIntersection();
		}

		private void ReloadCurrentIntersection()
		{
			if (_currentIntersectionSimulation is null)
			{
				return;
			}

			CleanUpTheSimulationData();

			UnitResult<Error> loadIntersectionResult = _simulationHandler.LoadIntersection(_currentIntersectionSimulation);

			if (loadIntersectionResult.IsFailure)
			{
				_logger.LogDebug("Simulation reloading failed [Error = {Error}]", loadIntersectionResult.Error);
				// TODO: Print error to the user
				return;
			}

			Intersection intersection = _simulationHandler.IntersectionSimulation!.Intersection;
			_currentIntersectionSimulation = _simulationHandler.IntersectionSimulation;

			DrawIntersection(intersection);
			UpdateIntersectionOptions(_currentIntersectionSimulation, SimulationFilePath);
		}

		private void CreateAndConfigureSimulationHandler()
		{
			_simulationHandler = _intersectionSimulationHandlerFactory.CreateHandler(SimulationModeName);
			_simulationHandler.SimulationUpdated += OnSimulationUpdated;

			if (_currentIntersectionSimulation is not null)
			{
				// TODO: Handle
				_ = _simulationHandler.LoadIntersection(_currentIntersectionSimulation);
			}
		}

		private Task AbortSimmulation()
		{
			_simulationHandler.Abort();

			return Task.CompletedTask;
		}

		private void ChangeSimulationMode()
		{
			ChangeSimulationModeName();

			_simulationHandler.Dispose();
			_simulationHandler = null;

			CreateAndConfigureSimulationHandler();
		}

		private void ChangeSimulationModeName()
		{
			if (SimulationModeName == SimulationMode.InMemory)
			{
				SimulationModeName = SimulationMode.RealTime;
			}
			else if (SimulationModeName == SimulationMode.RealTime)
			{
				SimulationModeName = SimulationMode.InMemory;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private async Task RunSimulation()
		{
			switch (SimulationModeName)
			{
				case SimulationMode.InMemory:
					await RunInMemorySimulation();
					break;
				case SimulationMode.RealTime:
					await RunRealTimeSimulation();
					break;
				default:
					throw new ArgumentException($"Unrecognized simulation mode [SimulationMode = {SimulationModeName}");
			}
		}
		private async Task RunInMemorySimulation()
		{
			CleanUpTheSimulationData();
			StartSimulationTimer();

			UnitResult<Error> startResult = await _simulationHandler.Start();

			if (startResult.IsFailure)
			{
				_logger.LogError("Simulation failed to complete [ErrorMessage = {ErrorMessage}", startResult.Error);
				StopSimulationTimer();
				return;
			}

			GetherSimulationResults();
			StopSimulationTimer();
		}

		private async Task RunRealTimeSimulation()
		{
			CleanUpTheSimulationData();
			StartSimulationTimer();

			UnitResult<Error> startResult = await _simulationHandler.Start();

			if (startResult.IsFailure)
			{
				_logger.LogError("Simulation failed to start [ErrorMessage = {ErrorMessage}", startResult.Error);
				StopSimulationTimer();
				return;
			}

			while (_simulationHandler.SimulationState.SimulationPhase
				is SimulationPhase.InProgress or SimulationPhase.InProgressCarGenerationFinished)
			{

				await Task.Delay(200);
			}

			GetherSimulationResults();
			StopSimulationTimer();
		}

		private void GetherSimulationResults()
		{
			SimulationResults simulationResults = _simulationHandler.SimulationResults;
			StepsTaken = simulationResults.SimulationStepsTaken;
			CarsPassed = simulationResults.CarsPassed;
			TotalCarsIdleTimeMs = simulationResults.TotalCarsIdleTimeMs;
			CalculationTimeSeconds = Math.Round(
				simulationResults.SimulationStepsTaken *
				_simulationHandler.IntersectionSimulation!.Options.StepTimespan.TotalSeconds, 2);
		}

		private void StopSimulationTimer()
		{
			_simulationTimer.Stop();
		}

		private void StartSimulationTimer()
		{
			_simulationStartTime = DateTime.Now;
			_simulationTimer.Start();
		}

		private void OnSimulationUpdated(object? sender, SimulationStateEventArgs e)
		{
			SimulationStepCounter = e.SimulationStep;
			CurrentTrafficPhaseItem = TrafficPhaseItems.Single(t => t.Name == e.CurrentTrafficPhaseName);

			foreach (KeyValuePair<Guid, TrafficLightState> trafficLights in e.TrafficLightsState)
			{
				TrafficLightUpdated?.Invoke(trafficLights.Key, trafficLights.Value);
			}

			foreach (KeyValuePair<Guid, CarLocation> carLocation in e.CarLocations)
			{
				CarLocationUpdated?.Invoke(carLocation.Key, carLocation.Value);
			}
		}

		private void CleanUpTheSimulationData()
		{
			NewSimulationStarted?.Invoke();

			StepsTaken = 0;
			CarsPassed = 0;
			TotalCarsIdleTimeMs = 0;
			CalculationTimeSeconds = 0;
			SimulationStepCounter = 0;

			if (_currentIntersectionSimulation is not null &&
				_currentIntersectionSimulation.SimulationState.SimulationPhase != SimulationPhase.NotStarted)
			{
				_currentIntersectionSimulation.Reset();
			}
		}

		private void SetInitialSimulationOptions(SimulationOptions options)
		{
			CanvasOptions.CarGeneratorsAreaOffset = options.CarGenerationAreaSize;
			CanvasOptions.CarWidth = options.CarSize;
			SimulationModeName = options.SimulationModeType;
			CurrentTrafficLightsModeItem = TrafficLightsModeItems.Single(i => i.Name == options.TrafficLightsMode);
		}

		private void SimulationTimerTick(object? sender, EventArgs e)
		{
			var elapsed = DateTime.Now - _simulationStartTime;
			ElapsedSimulationTime = elapsed.ToString(@"mm\:ss\:fff");
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
				_logger.LogError("Simulation loading failed [Error = {Error}]", loadIntersectionResult.Error);
				// TODO: Print error to the user
				return;
			}

			Intersection intersection = _simulationHandler.IntersectionSimulation!.Intersection;
			_currentIntersectionSimulation = _simulationHandler.IntersectionSimulation;

			DrawIntersection(intersection);
			UpdateIntersectionOptions(_currentIntersectionSimulation, jsonConfigurationFile);
		}

		private void UpdateIntersectionOptions(IntersectionSimulation intersectionSimulation, string jsonConfigurationFile)
		{
			SimulationName = intersectionSimulation.Name;
			SimulationId = intersectionSimulation.Id.ToString();
			SimulationFilePath = jsonConfigurationFile;
			SimulationTimespanMs = (int)intersectionSimulation.Options.StepTimespan.TotalMilliseconds;
			MinimalDistanceBetweenCars = 0;

			if (intersectionSimulation.Options.TrafficLightHandlerType is not null)
			{
				// TODO: Handle exception when wrong mode name is provided by configuration
				CurrentTrafficLightsModeItem = TrafficLightsModeItems.Single(i => i.Name == intersectionSimulation.Options.TrafficLightHandlerType);
			}

			TrafficPhaseItems.Clear();
			foreach (TrafficPhase trafficPhase in intersectionSimulation.Intersection.TrafficPhases)
			{
				TrafficPhaseItems.Add(trafficPhase.ToTrafficPhaseItem());
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
					IsInbound = true,
					ReferenceLaneId = lane.Id,
					TrafficLightsId = lane.TrafficLights?.Id,
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
					IsInbound = false,
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
					IsInbound = true,
					ReferenceLaneId = lane.Id,
					TrafficLightsId = lane.TrafficLights?.Id,
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
					IsInbound = false,
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
					IsInbound = true,
					ReferenceLaneId = lane.Id,
					TrafficLightsId = lane.TrafficLights?.Id,
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
					IsInbound = false,
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
					IsInbound = true,
					ReferenceLaneId = lane.Id,
					TrafficLightsId = lane.TrafficLights?.Id,
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
					IsInbound = false,
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
	}
}
