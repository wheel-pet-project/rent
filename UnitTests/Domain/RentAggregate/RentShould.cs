using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace UnitTests.Domain.RentAggregate;

[TestSubject(typeof(Rent))]
public class RentShould
{
    private readonly Booking _booking;
    private readonly Customer _customer;
    private readonly Vehicle _vehicle;
    private readonly VehicleModel _vehicleModel;
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    public RentShould()
    {
        var vehicleId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        
        _booking = Booking.Create(Guid.NewGuid(), vehicleId, customerId);
        _customer = Customer.Create(customerId);
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 400.0M, 2000.0M));
        _vehicle = Vehicle.Create(vehicleId, _vehicleModel);
    }
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEqual(Guid.Empty, actual.Id);
        Assert.Equal(_booking.Id, actual.BookingId);
        Assert.Equal(_customer.Id, actual.CustomerId);
        Assert.Equal(_vehicle.Id, actual.VehicleId);
        Assert.Equal(_vehicleModel.Tariff, actual.Tariff);
        Assert.NotEqual(default, actual.Start);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfBookingIsNull()
    {
        // Arrange

        // Act
        void Act() => Rent.Create(null!, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCustomerIsNull()
    {
        // Arrange

        // Act
        void Act() => Rent.Create(_booking, null!, _vehicle, _vehicleModel, _timeProvider);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIsNull()
    {
        // Arrange

        // Act
        void Act() => Rent.Create(_booking, _customer, null!, _vehicleModel, _timeProvider);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleModelIsNull()
    {
        // Arrange
        
        // Act
        void Act() => Rent.Create(_booking, _customer, _vehicle, null!, _timeProvider);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        // Arrange

        // Act
        void Act() => Rent.Create(_booking, _customer, _vehicle, _vehicleModel, null!);
        
        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void GetCurrentAmountReturn50ForStartedRent()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Act
        var actual = rent.GetCurrentAmount(_timeProvider);

        // Assert
        Assert.Equal(50, actual);
    }

    [Fact]
    public void GetCurrentAmountReturn50Plus10Minutes()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);
        var fakeTimeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow.AddMinutes(10));

        // Act
        var actual = rent.GetCurrentAmount(fakeTimeProvider);

        // Assert
        Assert.Equal(50 + 10 * _vehicleModel.Tariff.PricePerMinute, actual);
    }

    [Fact]
    public void GetCurrentAmountReturnAmountForDiffStartAndEndIfRentEnded()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);
        var fakeTimeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow.AddMinutes(10));
        rent.Complete(fakeTimeProvider);
        
        fakeTimeProvider.SetUtcNow(DateTimeOffset.UtcNow.AddHours(1));

        // Act
        var actual = rent.GetCurrentAmount(fakeTimeProvider);

        // Assert
        Assert.Equal(50 + 10 * _vehicleModel.Tariff.PricePerMinute, actual);
    }

    [Fact]
    public void CompleteSetEndProperty()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Act
        rent.Complete(_timeProvider);

        // Assert
        Assert.NotNull(rent.End);
    }

    [Fact]
    public void CompleteSetActualAmount()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Act
        rent.Complete(_timeProvider);

        // Assert
        Assert.NotNull(rent.ActualAmount);
    }

    [Fact]
    public void CompleteAddDomainEvent()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);
        rent.ClearDomainEvents();

        // Act
        rent.Complete(_timeProvider);

        // Assert
        Assert.NotEmpty(rent.DomainEvents);
    }

    [Fact]
    public void CompleteThrowValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        // Arrange
        var rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        // Act
        void Act() => rent.Complete(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}