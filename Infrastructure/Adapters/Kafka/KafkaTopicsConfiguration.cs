namespace Infrastructure.Adapters.Kafka;

public class KafkaTopicsConfiguration
{
    public required string VehicleCheckedTopic { get; set; }
    public required string VehicleCheckingStartedTopic { get; set; }
}