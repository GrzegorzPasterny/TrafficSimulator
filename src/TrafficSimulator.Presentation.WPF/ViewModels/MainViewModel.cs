using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models.IntersectionObjects;
using TrafficSimulator.Presentation.WPF.Helpers;

namespace TrafficSimulator.Presentation.WPF.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly ISimulationHandler _simulationHandler;
		private readonly ILogger<MainViewModel> _logger;

		public ObservableCollection<TrafficLights> TrafficLights { get; private set; } = new();

		[ObservableProperty]
		private ObservableCollection<UIElement> _intersectionElements = new();

		public ICommand LoadSimulationCommand { get; }

		public MainViewModel(ISimulationHandler simulationHandler, ILogger<MainViewModel> logger)
		{
			_simulationHandler = simulationHandler;

			LoadSimulationCommand = new RelayCommand(LoadIntersection);

			_logger = logger;
			_logger.LogInformation("MainViewModel initialized");
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

			DrawIntersection();
		}

		private void DrawIntersection()
		{
			IntersectionElements.Clear();

			var intersectionCore = new Rectangle
			{
				Width = 100,
				Height = 100,
				Fill = Brushes.DarkGray
			};

			Canvas.SetLeft(intersectionCore, 250);
			Canvas.SetTop(intersectionCore, 250);
			IntersectionElements.Add(intersectionCore);
		}
	}
}
