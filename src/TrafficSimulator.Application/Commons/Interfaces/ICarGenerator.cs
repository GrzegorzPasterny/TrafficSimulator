using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	// TODO: Should be domain object
	public interface ICarGenerator
	{
		UnitResult<Error> StartGenerating();
		UnitResult<Error> StopGenerating();
		ErrorOr<bool> IsGenerationFinished();
	}
}
