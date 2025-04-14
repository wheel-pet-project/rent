using From.DrivingLicenseKafkaEvents;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class DrivingLicenseApprovedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<DrivingLicenseApproved>
{
    public async Task Consume(ConsumeContext<DrivingLicenseApproved> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var consumerEvent = new DrivingLicenseApprovedConsumerEvent(
            @event.EventId,
            @event.AccountId);

        var isSaved = await inbox.Save(consumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}