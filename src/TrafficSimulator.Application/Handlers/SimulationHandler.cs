using CSharpFunctionalExtensions;
using ErrorOr;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.Models;

namespace TrafficSimulator.Application.Handlers
{
	internal class SimulationHandler : ISimulationHandler
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

		public ErrorOr<object> GetState()
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> LoadIntersection(Intersection intersection)
		{
			throw new NotImplementedException();
		}

		public UnitResult<Error> Start()
		{
			throw new NotImplementedException();
		}
	}
}
