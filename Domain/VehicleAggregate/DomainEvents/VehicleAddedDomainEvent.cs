using Domain.SharedKernel;

namespace Domain.VehicleAggregate.DomainEvents;

public record VehicleAddedDomainEvent : DomainEvent
{
    public VehicleAddedDomainEvent(Guid sagaId, Guid vehicleId)
    {
        if (sagaId == Guid.Empty) throw new ArgumentException($"{nameof(sagaId)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ArgumentException($"{nameof(vehicleId)} cannot be empty");

        SagaId = sagaId;
        VehicleId = vehicleId;
    }

    public Guid SagaId { get; }
    public Guid VehicleId { get; }
}