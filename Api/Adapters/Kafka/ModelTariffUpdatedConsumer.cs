using From.VehicleFleetKafkaEvents.Model;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class ModelTariffUpdatedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<ModelTariffUpdated>
{
    public async Task Consume(ConsumeContext<ModelTariffUpdated> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var modelTariffUpdatedEvent = new ModelTariffUpdatedConsumerEvent(
            @event.EventId,
            @event.ModelId,
            @event.PricePerMinute,
            @event.PricePerHour,
            @event.PricePerDay);

        var isSaved = await inbox.Save(modelTariffUpdatedEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}