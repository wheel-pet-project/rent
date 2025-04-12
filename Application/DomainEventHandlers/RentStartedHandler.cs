using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class RentStartedHandler(IMessageBus messageBus) : INotificationHandler<RentStartedDomainEvent>
{
    public async Task Handle(RentStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await messageBus.Publish(domainEvent, cancellationToken);
    }
}