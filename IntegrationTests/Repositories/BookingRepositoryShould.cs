using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(BookingRepository))]
public class BookingRepositoryShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel;
    private readonly Vehicle _vehicle;
    private readonly Customer _customer;
    private readonly Booking _booking;

    public BookingRepositoryShould()
    {
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _customer = Customer.Create(Guid.NewGuid());
        _booking = Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
    }

    [Fact]
    public async Task Add()
    {
        // Arrange
        await AddNeededAggregates();
        var (uow, repository) = Build(Context);

        // Act
        await repository.Add(_booking);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithBookingFromDb(_booking);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        await AddBookingToDatabase();
        var (_, repository) = Build(Context);

        // Act
        var actual = await repository.GetById(_booking.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_booking, actual);
    }

    private async Task AddBookingToDatabase()
    {
        await AddNeededAggregates();

        await Context.Bookings.AddAsync(_booking);
        await Context.SaveChangesAsync();
    }

    private async Task AddNeededAggregates()
    {
        await Context.VehicleModels.AddAsync(_vehicleModel);
        await Context.SaveChangesAsync();

        await Context.Vehicles.AddAsync(_vehicle);
        Context.Attach(_customer.Level);
        await Context.Customers.AddAsync(_customer);
        await Context.SaveChangesAsync();
    }

    private async Task AssertEquivalentWithBookingFromDb(Booking expected)
    {
        Context.ChangeTracker.Clear();
        var bookingFromDb = await Context.Bookings.FirstOrDefaultAsync(x => x.Id == _booking.Id);
        Assert.Equivalent(expected, bookingFromDb);
    }

    private (Infrastructure.Adapters.Postgres.UnitOfWork, BookingRepository) Build(DataContext context)
    {
        return (new Infrastructure.Adapters.Postgres.UnitOfWork(context), new BookingRepository(context));
    }
}