using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public interface ICarGenerator
	{
		UnitResult<Error> StartGenerating();
		UnitResult<Error> StopGenerating();
		ErrorOr<bool> IsGenerationFinished();
	}
}
