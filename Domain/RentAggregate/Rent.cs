using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate.DomainEvents;
using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;

namespace Domain.RentAggregate;

public class Rent : Aggregate
{
    private Rent()
    {
    }

    private Rent(Guid bookingId, Guid customerId, Guid vehicleId, Tariff tariff, DateTime start) : this()
    {
        Id = Guid.NewGuid();
        Status = Status.InProgress;
        BookingId = bookingId;
        CustomerId = customerId;
        VehicleId = vehicleId;
        Tariff = tariff;
        Start = start;
    }

    public Guid Id { get; }
    public Status Status { get; private set; } = null!;
    public Guid BookingId { get; }
    public Guid CustomerId { get; }
    public Guid VehicleId { get; }
    public Tariff Tariff { get; } = null!;
    public DateTime Start { get; }
    public DateTime? End { get; private set; }
    public decimal? ActualAmount { get; private set; }

    public decimal GetCurrentAmount(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");

        var currentAmount = CalculateCurrentAmount();
        var roundedAmount = RoundAmount(currentAmount);

        return roundedAmount;

        decimal CalculateCurrentAmount()
        {
            const int minAmount = 50;

            var amount = End == null
                ? CalculateAmountForNotCompletedRent()
                : CalculateAmountForCompletedRent();

            return amount + minAmount;
        }

        decimal CalculateAmountForNotCompletedRent()
        {
            return Tariff.PricePerMinute * (decimal)(timeProvider.GetUtcNow().UtcDateTime - Start).TotalMinutes;
        }

        decimal CalculateAmountForCompletedRent()
        {
            return Tariff.PricePerMinute * (decimal)(End - Start).Value.TotalMinutes;
        }

        decimal RoundAmount(decimal amount)
        {
            return decimal.Round(amount, 2);
        }
    }

    public decimal Complete(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");

        if (Status.CanBeChangedToThisStatus(Status.Completed) == false)
            throw new DomainRulesViolationException(
                "Rent cannot be completed");

        var actualAmount = GetCurrentAmount(timeProvider);

        End = timeProvider.GetUtcNow().UtcDateTime;
        ActualAmount = actualAmount;
        Status = Status.Completed;

        AddDomainEvent(new RentCompletedDomainEvent(Id, BookingId, VehicleId, CustomerId, (double)actualAmount));

        return actualAmount;
    }

    public static Rent Create(
        Booking booking,
        Customer customer,
        Vehicle vehicle,
        VehicleModel vehicleModel,
        TimeProvider timeProvider)
    {
        if (booking == null) throw new ValueIsRequiredException($"{nameof(booking)} cannot be null");
        if (customer == null) throw new ValueIsRequiredException($"{nameof(customer)} cannot be null");
        if (vehicle == null) throw new ValueIsRequiredException($"{nameof(vehicle)} cannot be null");
        if (vehicleModel == null) throw new ValueIsRequiredException($"{nameof(vehicleModel)} cannot be null");
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");

        if (booking.CustomerId != customer.Id || booking.VehicleId != vehicle.Id ||
            vehicle.VehicleModelId != vehicleModel.Id)
            throw new DomainRulesViolationException("Id mismatch for creating rent");

        var rent = new Rent(booking.Id, customer.Id, vehicle.Id, Tariff.With(vehicleModel.Tariff),
            timeProvider.GetUtcNow().UtcDateTime);

        rent.AddDomainEvent(new RentStartedDomainEvent(rent.Id, rent.BookingId, rent.VehicleId, rent.CustomerId));

        return rent;
    }
}