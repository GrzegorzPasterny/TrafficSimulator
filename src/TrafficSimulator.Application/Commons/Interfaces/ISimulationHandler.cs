using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Domain.Models;
using TrafficSimulator.Domain.Simulation;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler : IDisposable
{
	SimulationState SimulationState { get; }
	SimulationResults SimulationResults { get; }
	IntersectionSimulation? IntersectionSimulation { get; }

	UnitResult<Error> LoadIntersection(IntersectionSimulation intersectionSimulation);

	/// <summary>
	/// Loads Intersection based on <paramref name="identifier"/>
	/// </summary>
	/// <param name="identifier">Simulation configuration id, or filePath</param>
	/// <returns></returns>
	Task<UnitResult<Error>> LoadIntersection(string identifier);

	Task<UnitResult<Error>> Start();

	UnitResult<Error> Abort();

}

