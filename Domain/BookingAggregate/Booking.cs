using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Domain.BookingAggregate;

public sealed class Booking
{
    private Booking()
    {
    }

    private Booking(Guid id, Guid vehicleId, Guid customerId) : this()
    {
        Id = id;
        VehicleId = vehicleId;
        CustomerId = customerId;
    }

    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CustomerId { get; private set; }

    public static Booking Create(Guid id, Guid vehicleId, Guid customerId)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(customerId)} cannot be empty");

        return new Booking(id, vehicleId, customerId);
    }
}