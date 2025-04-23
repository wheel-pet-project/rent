using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using Domain.SharedKernel;
using Domain.VehicleAggregate.DomainEvents;
using From.RentKafkaEvents;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Infrastructure.Adapters.Kafka;

public class KafkaProducer(
    ITopicProducerProvider topicProducerProvider,
    IOptions<KafkaTopicsConfiguration> configuration) : IMessageBus
{
    private readonly KafkaTopicsConfiguration _configuration = configuration.Value;

    public async Task Publish(VehicleAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, VehicleAddingToRentProcessed>(
            new Uri($"topic:{_configuration.VehicleAddingToRentProcessedTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new VehicleAddingToRentProcessed(
                domainEvent.EventId,
                domainEvent.SagaId,
                domainEvent.VehicleId,
                true),
            SetMessageId<VehicleAddingToRentProcessed, VehicleAddedDomainEvent>(domainEvent), cancellationToken);
    }

    public async Task Publish(RentStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, RentStarted>(
            new Uri($"topic:{_configuration.RentStartedTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new RentStarted(
                domainEvent.EventId,
                domainEvent.RentId,
                domainEvent.BookingId,
                domainEvent.VehicleId,
                domainEvent.CustomerId),
            SetMessageId<RentStarted, RentStartedDomainEvent>(domainEvent), cancellationToken);
    }

    public async Task Publish(RentCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, RentCompleted>(
            new Uri($"topic:{_configuration.RentCompletedTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new RentCompleted(
                domainEvent.EventId,
                domainEvent.RentId,
                domainEvent.BookingId,
                domainEvent.VehicleId,
                domainEvent.CustomerId,
                domainEvent.ActualAmount),
            SetMessageId<RentCompleted, RentCompletedDomainEvent>(domainEvent), cancellationToken);
    }

    private IPipe<KafkaSendContext<string, TContractEvent>> SetMessageId<TContractEvent, TDomainEvent>(
        TDomainEvent domainEvent)
        where TDomainEvent : DomainEvent
        where TContractEvent : class
    {
        return Pipe.Execute<KafkaSendContext<string, TContractEvent>>(ctx => ctx.MessageId = domainEvent.EventId);
    }
}