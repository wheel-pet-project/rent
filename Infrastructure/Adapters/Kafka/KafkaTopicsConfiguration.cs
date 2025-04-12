namespace Infrastructure.Adapters.Kafka;

public class KafkaTopicsConfiguration
{
    public required string VehicleAddingToRentProcessedTopic { get; set; }
    public required string VehicleAddedTopic { get; set; }
    public required string VehicleDeletedTopic { get; set; }
    public required string RentStartedTopic { get; set; }
    public required string RentCompletedTopic { get; set; }
}