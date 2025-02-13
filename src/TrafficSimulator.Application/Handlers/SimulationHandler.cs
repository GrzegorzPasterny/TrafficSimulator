using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Handlers
{
	public class SimulationHandler : ISimulationHandler
	{
		private readonly IIntersectionRepository _intersectionRepository;

		public SimulationHandler(IIntersectionRepository intersectionRepository)
		{
			_intersectionRepository = intersectionRepository;
		}

		public UnitResult<Error> Abort()
		{
			throw new NotImplementedException();
		}

		public ErrorOr<SimulationState> GetState()
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> LoadIntersection(Intersection intersection)
		{
			return _intersectionRepository.SetCurrentIntersection(intersection);
		}

		/// <summary>
		/// Starts simulation
		/// </summary>
		/// <remarks>
		///		Starts Car generators.
		///		Initialize Traffic Lights controller
		/// </remarks>
		public UnitResult<Error> Start()
		{
			throw new NotImplementedException();
		}
	}
}
