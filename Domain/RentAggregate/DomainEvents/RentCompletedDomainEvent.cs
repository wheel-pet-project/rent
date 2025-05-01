using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Domain.RentAggregate.DomainEvents;

public record RentCompletedDomainEvent : DomainEvent
{
    public RentCompletedDomainEvent(Guid rentId, Guid bookingId, Guid vehicleId, Guid customerId, double actualAmount)
    {
        if (rentId == Guid.Empty) throw new ArgumentException($"{nameof(rentId)} cannot be empty");
        if (bookingId == Guid.Empty) throw new ArgumentException($"{nameof(bookingId)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ArgumentException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ArgumentException($"{nameof(customerId)} cannot be empty");
        if (actualAmount <= 0)
            throw new ArgumentException(
                $"{nameof(actualAmount)} cannot be equal or less than zero");

        RentId = rentId;
        BookingId = bookingId;
        VehicleId = vehicleId;
        CustomerId = customerId;
        ActualAmount = actualAmount;
    }

    public Guid RentId { get; }
    public Guid BookingId { get; }
    public Guid VehicleId { get; }
    public Guid CustomerId { get; }
    public double ActualAmount { get; }
}