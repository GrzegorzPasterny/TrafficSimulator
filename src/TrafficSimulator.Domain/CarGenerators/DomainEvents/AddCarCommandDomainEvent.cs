using MediatR;
using TrafficSimulator.Domain.Models.Agents;

namespace TrafficSimulator.Domain.CarGenerators.DomainEvents
{
	public record AddCarCommandDomainEvent(Car Car) : IRequest;
}
