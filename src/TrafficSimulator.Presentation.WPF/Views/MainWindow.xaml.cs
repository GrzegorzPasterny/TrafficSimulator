using System.Windows;
using TrafficSimulator.Presentation.WPF.ViewModels;

namespace TrafficSimulator.Presentation.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow(MainViewModel mainViewModel)
	{
		InitializeComponent();
		DataContext = mainViewModel;
	}
}