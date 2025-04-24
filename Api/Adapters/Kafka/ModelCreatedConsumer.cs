using From.VehicleFleetKafkaEvents.Model;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class ModelCreatedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<ModelCreated>
{
    public async Task Consume(ConsumeContext<ModelCreated> context)
    {
        var @event = context.Message;
        var consumerEvent = new ModelCreatedConsumerEvent(
            @event.EventId,
            @event.ModelId,
            @event.PricePerMinute,
            @event.PricePerHour,
            @event.PricePerDay);

        var isSaved = await inbox.Save(consumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}