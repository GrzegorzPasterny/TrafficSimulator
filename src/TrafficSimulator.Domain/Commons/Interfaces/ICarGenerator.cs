using CSharpFunctionalExtensions;
using ErrorOr;
using System.Text.Json.Serialization;
using TrafficSimulator.Domain.CarGenerators;

namespace TrafficSimulator.Domain.Commons.Interfaces
{
	public interface ICarGenerator
	{
		[JsonIgnore]
		CarGeneratorOptions Options { get; }

		Task<UnitResult<Error>> Generate(TimeSpan timeSpan);
		bool IsGenerationFinished { get; }
		void Reset();
	}
}
