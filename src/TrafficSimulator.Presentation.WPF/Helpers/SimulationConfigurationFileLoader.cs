using Microsoft.Win32;
using System.IO;

namespace TrafficSimulator.Presentation.WPF.Helpers
{
	public class SimulationConfigurationFileLoader
	{
		/// <summary>
		/// Returns json file path, or null if user cancelled the dialog
		/// </summary>
		/// <returns></returns>
		public static string? LoadFromJsonFile()
		{
			var dialog = new OpenFileDialog
			{
				InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Configurations"),
				Filter = "JSON Files (*.json)|*.json",
				Title = "Select Simulation Configuration"
			};

			if (dialog.ShowDialog() == false)
			{
				return null;
			}

			return dialog.FileName;
		}
	}
}
