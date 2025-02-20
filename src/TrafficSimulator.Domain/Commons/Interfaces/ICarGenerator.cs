using CSharpFunctionalExtensions;
using ErrorOr;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public interface ICarGenerator
	{
		Task<UnitResult<Error>> Generate(TimeSpan timeSpan);
		bool IsGenerationFinished { get; }
	}
}
