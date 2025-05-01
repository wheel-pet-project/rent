using Domain.BookingAggregate;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.BookingAggregate;

[TestSubject(typeof(Booking))]
public class BookingShould
{
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange


        // Act
        var actual = Booking.Create(_bookingId, _vehicleId, _customerId);

        // Assert
        Assert.Equal(_bookingId, actual.Id);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.Equal(_customerId, actual.CustomerId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfBookingIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            Booking.Create(Guid.Empty, _vehicleId, _customerId);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            Booking.Create(_bookingId, Guid.Empty, _customerId);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCustomerIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            Booking.Create(_bookingId, _vehicleId, Guid.Empty);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}