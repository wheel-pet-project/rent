using From.BookingKafkaEvents;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class BookingCompletedConsumer(IInbox inbox) : IConsumer<BookingCompleted>
{
    public async Task Consume(ConsumeContext<BookingCompleted> context)
    {
        var @event = context.Message;
        var consumerEvent = new BookingCompletedConsumerEvent(
            @event.EventId,
            @event.BookingId,
            @event.VehicleId,
            @event.CustomerId);

        var isSaved = await inbox.Save(consumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}