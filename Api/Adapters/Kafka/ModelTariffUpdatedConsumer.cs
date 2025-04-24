using From.VehicleFleetKafkaEvents.Model;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class ModelTariffUpdatedConsumer(IInbox inbox) : IConsumer<ModelTariffUpdated>
{
    public async Task Consume(ConsumeContext<ModelTariffUpdated> context)
    {
        var @event = context.Message;
        var consumerEvent = new ModelTariffUpdatedConsumerEvent(
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