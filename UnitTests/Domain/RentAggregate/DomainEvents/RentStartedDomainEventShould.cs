using Domain.RentAggregate.DomainEvents;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.RentAggregate.DomainEvents;

[TestSubject(typeof(RentStartedDomainEvent))]
public class RentStartedDomainEventShould
{
    private readonly Guid _rentId = Guid.NewGuid();
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new RentStartedDomainEvent(_rentId, _bookingId, _vehicleId, _customerId);

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
            new RentStartedDomainEvent(Guid.Empty, _bookingId, _vehicleId, _customerId);
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
            new RentStartedDomainEvent(_rentId, Guid.Empty, _vehicleId, _customerId);
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
            new RentStartedDomainEvent(_rentId, _bookingId, Guid.Empty, _customerId);
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
            new RentStartedDomainEvent(_rentId, _bookingId, _vehicleId, Guid.Empty);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}