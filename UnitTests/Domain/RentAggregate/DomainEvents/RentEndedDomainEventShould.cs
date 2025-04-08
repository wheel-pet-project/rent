using Domain.RentAggregate.DomainEvents;
using Domain.SharedKernel.Exceptions.ArgumentException;
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
        var actual = new RentEndedDomainEvent(_rentId, _bookingId, _vehicleId, _customerId, _totalAmount);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_rentId, actual.RentId);
        Assert.Equal(_bookingId, actual.BookingId);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.Equal(_customerId, actual.CustomerId);
    }
    
    [Fact]
    public void ThrowValueIsRequiredExceptionWhenRentIdIsNull()
    {
        // Arrange

        // Act
        void Act() => new RentEndedDomainEvent(Guid.Empty, _bookingId, _vehicleId, _customerId, _totalAmount);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionWhenBookingIdIsNull()
    {
        // Arrange

        // Act
        void Act() => new RentEndedDomainEvent(_rentId, Guid.Empty, _vehicleId, _customerId, _totalAmount);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionWhenVehicleIdIsNull()
    {
        // Arrange

        // Act
        void Act() => new RentEndedDomainEvent(_rentId, _bookingId, Guid.Empty, _customerId, _totalAmount);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionWhenCustomerIdIsNull()
    {
        // Arrange

        // Act
        void Act() => new RentEndedDomainEvent(_rentId, _bookingId, _vehicleId, Guid.Empty, _totalAmount);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionWhenTotalAmountIsEqualOrLessThanZero()
    {
        // Arrange

        // Act
        void Act() => new RentEndedDomainEvent(_rentId, _bookingId, _vehicleId, _customerId, 0);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}