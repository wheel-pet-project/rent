using Application.UseCases.Queries.Rent.GetAllCurrentRents;
using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace IntegrationTests.Queries;

[TestSubject(typeof(GetAllCurrentRentsQueryHandler))]
public class GetAllCurrentRentsQueryHandlerShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel;
    private readonly Vehicle _vehicle;
    private readonly Customer _customer;
    private readonly Booking _booking;
    private readonly Rent _rent;
    private readonly GetAllCurrentRentsQuery _query = new(1, 10);


    public GetAllCurrentRentsQueryHandlerShould()
    {
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        _customer = Customer.Create(Guid.NewGuid());
        _booking = Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
        _rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, TimeProvider.System);
    }

    [Fact]
    public async Task ReturnEmptyListIfNoCurrentRents()
    {
        // Arrange
        await AddRent(_vehicleModel, _vehicle, _customer, _booking, _rent, true);

        var queryHandler = new GetAllCurrentRentsQueryHandler(DataSource);

        // Act
        var actual = await queryHandler.Handle(_query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Empty(actual.Value.Rents);
    }

    [Fact]
    public async Task ReturnCurrentRentsIfExistsWithCorrectValues()
    {
        // Arrange
        var expectedRent = await AddRent(_vehicleModel, _vehicle, _customer, _booking, _rent);

        var queryHandler = new GetAllCurrentRentsQueryHandler(DataSource);

        // Act
        var actual = await queryHandler.Handle(_query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotEmpty(actual.Value.Rents);
        Assert.Equal(expectedRent.Id, actual.Value.Rents[0].RentId);
        Assert.Equal(expectedRent.CustomerId, actual.Value.Rents[0].CustomerId);
        Assert.Equal(expectedRent.VehicleId, actual.Value.Rents[0].VehicleId);
        Assert.Equal(expectedRent.Start, actual.Value.Rents[0].Start);
    }

    private async Task<Rent> AddRent(
        VehicleModel vehicleModel,
        Vehicle vehicle,
        Customer customer,
        Booking booking,
        Rent rent,
        bool completeRent = false)
    {
        await Context.VehicleModels.AddAsync(vehicleModel);
        await Context.SaveChangesAsync();

        await Context.Vehicles.AddAsync(vehicle);
        Context.Attach(customer.Level);
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        await Context.Bookings.AddAsync(booking);
        await Context.SaveChangesAsync();


        if (completeRent) rent.Complete(TimeProvider.System);

        await Context.Rents.AddAsync(rent);
        await Context.SaveChangesAsync();

        return rent;
    }
}