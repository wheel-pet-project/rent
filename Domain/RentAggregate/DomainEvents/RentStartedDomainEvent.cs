using Domain.SharedKernel;

namespace Domain.RentAggregate.DomainEvents;

public record RentStartedDomainEvent : DomainEvent
{
    public RentStartedDomainEvent(Guid rentId, Guid bookingId, Guid vehicleId, Guid customerId)
    {
        if (rentId == Guid.Empty) throw new ArgumentException($"{nameof(rentId)} cannot be empty");
        if (bookingId == Guid.Empty) throw new ArgumentException($"{nameof(bookingId)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ArgumentException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ArgumentException($"{nameof(customerId)} cannot be empty");

        RentId = rentId;
        BookingId = bookingId;
        VehicleId = vehicleId;
        CustomerId = customerId;
    }

    public Guid RentId { get; }
    public Guid BookingId { get; }
    public Guid VehicleId { get; }
    public Guid CustomerId { get; }
}