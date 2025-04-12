using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class RentEndedHandler(IMessageBus messageBus) : INotificationHandler<RentCompletedDomainEvent>
{
    public async Task Handle(RentCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await messageBus.Publish(domainEvent, cancellationToken);
    }
}