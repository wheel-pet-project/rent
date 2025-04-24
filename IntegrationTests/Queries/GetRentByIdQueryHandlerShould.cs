using Application.UseCases.Queries.Rent.GetRentById;
using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace IntegrationTests.Queries;

[TestSubject(typeof(GetRentByIdQueryHandler))]
public class GetRentByIdQueryHandlerShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel;
    private readonly Vehicle _vehicle;
    private readonly Customer _customer;
    private readonly Booking _booking;
    private readonly Rent _rent;

    public GetRentByIdQueryHandlerShould()
    {
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _customer = Customer.Create(Guid.NewGuid());
        _booking = Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
        _rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, TimeProvider.System);
    }

    [Fact]
    public async Task ReturnRentWithCorrectValues()
    {
        // Arrange
        await AddCompletedRent(_vehicleModel, _vehicle, _customer, _booking, _rent);

        var query = new GetRentByIdQuery(_rent.Id);
        var queryHandler = new GetRentByIdQueryHandler(DataSource);

        // Act
        var actual = await queryHandler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual);
        Assert.Equal(_rent.Id, actual.Value.RentId);
        Assert.Equal(_rent.Status, actual.Value.Status);
        Assert.Equal(_rent.BookingId, actual.Value.BookingId);
        Assert.Equal(_rent.VehicleId, actual.Value.VehicleId);
        Assert.Equal(_rent.CustomerId, actual.Value.CustomerId);
        Assert.Equal(_rent.Start, actual.Value.Start);
        Assert.Equal(_rent.End, actual.Value.End);
        Assert.Equal(_rent.ActualAmount, actual.Value.ActualAmount);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfRentNotFound()
    {
        // Arrange
        var query = new GetRentByIdQuery(_rent.Id);
        var queryHandler = new GetRentByIdQueryHandler(DataSource);

        // Act
        var actual = await queryHandler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    private async Task<Rent> AddCompletedRent(
        VehicleModel vehicleModel,
        Vehicle vehicle,
        Customer customer,
        Booking booking,
        Rent rent)
    {
        await Context.VehicleModels.AddAsync(vehicleModel);
        await Context.SaveChangesAsync();

        await Context.Vehicles.AddAsync(vehicle);
        Context.Attach(customer.Level);
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        await Context.Bookings.AddAsync(booking);
        await Context.SaveChangesAsync();

        rent.Complete(TimeProvider.System);

        Context.Attach(rent.Status);
        await Context.Rents.AddAsync(rent);
        await Context.SaveChangesAsync();

        return rent;
    }
}