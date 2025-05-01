using Domain.RentAggregate.DomainEvents;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.RentAggregate.DomainEvents;

[TestSubject(typeof(RentEndedDomainEventShould))]
public class RentEndedDomainEventShould
{
    private readonly Guid _rentId = Guid.NewGuid();
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly double _totalAmount = 200.00;

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new RentCompletedDomainEvent(_rentId, _bookingId, _vehicleId, _customerId, _totalAmount);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_rentId, actual.RentId);
        Assert.Equal(_bookingId, actual.BookingId);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.Equal(_customerId, actual.CustomerId);
    }

    [Fact]
    public void ThrowArgumentExceptionWhenRentIdIsNull()
    {
        // Arrange

        // Act
        void Act()
        {
            new RentCompletedDomainEvent(Guid.Empty, _bookingId, _vehicleId, _customerId, _totalAmount);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ThrowArgumentExceptionWhenBookingIdIsNull()
    {
        // Arrange

        // Act
        void Act()
        {
            new RentCompletedDomainEvent(_rentId, Guid.Empty, _vehicleId, _customerId, _totalAmount);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ThrowArgumentExceptionWhenVehicleIdIsNull()
    {
        // Arrange

        // Act
        void Act()
        {
            new RentCompletedDomainEvent(_rentId, _bookingId, Guid.Empty, _customerId, _totalAmount);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ThrowArgumentExceptionWhenCustomerIdIsNull()
    {
        // Arrange

        // Act
        void Act()
        {
            new RentCompletedDomainEvent(_rentId, _bookingId, _vehicleId, Guid.Empty, _totalAmount);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ThrowArgumentExceptionWhenTotalAmountIsEqualOrLessThanZero()
    {
        // Arrange

        // Act
        void Act()
        {
            new RentCompletedDomainEvent(_rentId, _bookingId, _vehicleId, _customerId, 0);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}