using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Application.Commons.Interfaces
{
	public interface ICarGenerator
	{
		UnitResult<Error> StartGenerating();
		UnitResult<Error> StopGenerating();
		ErrorOr<bool> IsGenerationFinished();
	}
}
