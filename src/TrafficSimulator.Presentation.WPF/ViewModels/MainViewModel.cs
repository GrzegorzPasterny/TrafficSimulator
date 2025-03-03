using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Presentation.WPF.Helpers;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly ISimulationHandler _simulationHandler;
		private readonly ILogger<MainViewModel> _logger;

		public IntersectionElementsOptions CanvasOptions { get; } = new();

		[ObservableProperty]
		private ObservableCollection<TrafficElement> trafficElements = new();
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

			//DrawIntersection();
		}

		private void LoadDummyIntersection()
		{
			TrafficElements.Clear();

			// Intersection Core
			TrafficElements.Add(new TrafficElement(TrafficElementType.IntersectionCore, 250, 250, 100, 100, "DarkGray"));

			// Inbound Lanes
			TrafficElements.Add(new TrafficElement(TrafficElementType.Lane, 300, 0, 6, 250, "Black")); // North
			TrafficElements.Add(new TrafficElement(TrafficElementType.Lane, 300, 350, 6, 250, "Black")); // South
			TrafficElements.Add(new TrafficElement(TrafficElementType.Lane, 350, 300, 250, 6, "Black")); // East
			TrafficElements.Add(new TrafficElement(TrafficElementType.Lane, 0, 300, 250, 6, "Black")); // West

			// Traffic Lights
			TrafficElements.Add(new TrafficElement(TrafficElementType.TrafficLight, 290, 230, 20, 20, "Red")); // North
			TrafficElements.Add(new TrafficElement(TrafficElementType.TrafficLight, 290, 370, 20, 20, "Red")); // South
			TrafficElements.Add(new TrafficElement(TrafficElementType.TrafficLight, 370, 290, 20, 20, "Red")); // East
			TrafficElements.Add(new TrafficElement(TrafficElementType.TrafficLight, 230, 290, 20, 20, "Red")); // West
		}
	}

	public enum TrafficElementType
	{
		IntersectionCore,
		Lane,
		TrafficLight
	}

	public class TrafficElement
	{
		public TrafficElementType Type { get; }
		public double X { get; }
		public double Y { get; }
		public double Width { get; }
		public double Height { get; }
		public string Color { get; }

		public TrafficElement(TrafficElementType type, double x, double y, double width, double height, string color)
		{
			Type = type;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			Color = color;
		}
	}
}
