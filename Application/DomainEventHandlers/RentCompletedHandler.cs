using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class RentCompletedHandler(IMessageBus messageBus) : INotificationHandler<RentCompletedDomainEvent>
{
    public async Task Handle(RentCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // todo: add updating level in loyalty program
        await messageBus.Publish(domainEvent, cancellationToken);
    }
}