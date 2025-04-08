using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.RentAggregate.DomainEvents;

public record RentEndedDomainEvent : DomainEvent
{
    public RentEndedDomainEvent(Guid rentId, Guid bookingId, Guid vehicleId, Guid customerId, double totalPrice)
    {
        if (rentId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(rentId)} cannot be empty");
        if (bookingId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(bookingId)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(customerId)} cannot be empty");
        if (totalPrice <= 0) throw new ValueIsRequiredException(
            $"{nameof(totalPrice)} cannot be equal or less than zero");
        
        RentId = rentId;
        BookingId = bookingId;
        VehicleId = vehicleId;
        CustomerId = customerId;
        TotalPrice = totalPrice;
    }
    
    public Guid RentId { get; }
    public Guid BookingId { get; }
    public Guid VehicleId { get; }
    public Guid CustomerId { get; }
    public double TotalPrice { get; }
}