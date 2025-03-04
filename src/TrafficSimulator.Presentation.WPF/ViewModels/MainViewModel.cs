using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Commons;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Presentation.WPF.Helpers;
using TrafficSimulator.Presentation.WPF.ViewModels.IntersectionElements;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly ISimulationHandler _simulationHandler;
		private readonly ILogger<MainViewModel> _logger;

		public IntersectionElementsOptions CanvasOptions { get; } = new();

		[ObservableProperty]
		private IntersectionElement _intersectionElement = new();

		private IntersectionElement _tempIntersectionElement = new();

		public ICommand LoadSimulationCommand { get; }

		public MainViewModel(ISimulationHandler simulationHandler, ILogger<MainViewModel> logger)
		{
			_simulationHandler = simulationHandler;

			LoadSimulationCommand = new RelayCommand(LoadIntersection);

			_logger = logger;
			_logger.LogInformation("MainViewModel initialized");

			LoadDummyIntersection();
		}

		private void LoadIntersection()
		{
			string? jsonConfigurationFile = SimulationConfigurationFileLoader.LoadFromJsonFile();

			if (jsonConfigurationFile is null)
			{
				_logger.LogDebug("Attempt to select json configuration file was cancelled");
				return;
			}

			UnitResult<Error> loadIntersectionResult =
				Task.Run(() => _simulationHandler.LoadIntersection(jsonConfigurationFile))
				.GetAwaiter().GetResult();

			if (loadIntersectionResult.IsFailure)
			{
				_logger.LogDebug("Simulation loading failed [Error = {Error}]", loadIntersectionResult.Error);
				// TODO: Print error to the user
				return;
			}

			Intersection intersection = _simulationHandler.IntersectionSimulation!.Intersection;

			DrawIntersection(intersection);
		}

		private void DrawIntersection(Intersection intersection)
		{
			_tempIntersectionElement = new IntersectionElement();

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
			int northLanes = GetAmountOfLanes(intersection, WorldDirection.North);
			int southLanes = GetAmountOfLanes(intersection, WorldDirection.South);
			int westLanes = GetAmountOfLanes(intersection, WorldDirection.West);
			int eastLanes = GetAmountOfLanes(intersection, WorldDirection.East);


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
