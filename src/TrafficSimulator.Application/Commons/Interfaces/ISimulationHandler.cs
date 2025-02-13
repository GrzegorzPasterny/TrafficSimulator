using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Application.Commons.Interfaces;

public interface ISimulationHandler
{
	UnitResult<Error> Start();

	ErrorOr<object> GetState();



}

