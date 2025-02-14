namespace TrafficSimulator.Application.Commons.Interfaces
{
	// TODO: Should be domain object
	public interface ICarGeneratorRepository
	{
		Task AddCarGeneratorAsync(ICarGenerator carGenerator);

		Task<IEnumerable<ICarGenerator>> GetCarGeneratorsAsync();
	}
}
