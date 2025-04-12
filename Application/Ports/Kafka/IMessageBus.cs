using Domain.RentAggregate.DomainEvents;
using Domain.VehicleAggregate.DomainEvents;

namespace Application.Ports.Kafka;

public interface IMessageBus
{
    Task Publish(VehicleAddedDomainEvent domainEvent, CancellationToken cancellationToken);

    Task Publish(RentStartedDomainEvent domainEvent, CancellationToken cancellationToken);

    Task Publish(RentCompletedDomainEvent domainEvent, CancellationToken cancellationToken);
}