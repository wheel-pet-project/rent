using Application.UseCases.Queries.Rent.GetCurrentAmountRent;
using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Xunit;

namespace IntegrationTests.Queries;

[TestSubject(typeof(GetCurrentAmountRentQueryHandler))]
public class GetCurrentAmountRentQueryHandlerShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel;
    private readonly Vehicle _vehicle;
    private readonly Customer _customer;
    private readonly Booking _booking;
    private readonly Rent _rent;

    public GetCurrentAmountRentQueryHandlerShould()
    {
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _customer = Customer.Create(Guid.NewGuid());
        _booking = Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
        _rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, TimeProvider.System);
    }

    [Fact]
    public async Task ReturnCurrentAmountRent()
    {
        // Arrange
        await AddRent(_vehicleModel, _vehicle, _customer, _booking, _rent);

        var query = new GetCurrentAmountRentQuery(_rent.Id);
        var queryHandler = new GetCurrentAmountRentQueryHandler(new RentRepository(Context), TimeProvider.System);

        // Act
        var actual = await queryHandler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotNull(actual);
        Assert.Equal(_rent.Id, actual.Value.RentId);
        Assert.True(actual.Value.CurrentAmount > 0);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfRentNotFound()
    {
        var query = new GetCurrentAmountRentQuery(_rent.Id);
        var queryHandler = new GetCurrentAmountRentQueryHandler(new RentRepository(Context), TimeProvider.System);

        // Act
        var actual = await queryHandler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    private async Task<Rent> AddRent(
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

        Context.Attach(rent.Status);
        await Context.Rents.AddAsync(rent);
        await Context.SaveChangesAsync();

        return rent;
    }
}