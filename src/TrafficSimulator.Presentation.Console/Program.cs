using ConsoleAppFramework;
using TrafficSimulator.Presentation.Console.Controllers;

var app = ConsoleApp.Create();
app.Add<IntersectionSimulationController>();
app.Run(args);

