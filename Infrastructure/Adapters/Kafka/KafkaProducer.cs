using Application.Ports.Kafka;
using Domain.SharedKernel;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Infrastructure.Adapters.Kafka;

public class KafkaProducer(
    ITopicProducerProvider topicProducerProvider,
    IOptions<KafkaTopicsConfiguration> configuration) : IMessageBus
{
    private readonly KafkaTopicsConfiguration _configuration = configuration.Value;
    
    private IPipe<KafkaSendContext<string, TContractEvent>> SetMessageId<TContractEvent, TDomainEvent>(
        TDomainEvent domainEvent)
        where TDomainEvent : DomainEvent
        where TContractEvent : class
    {
        return Pipe.Execute<KafkaSendContext<string, TContractEvent>>(ctx => ctx.MessageId = domainEvent.EventId);
    }
}