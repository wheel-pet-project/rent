using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.RentAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(RentRepository))]
public class RentRepositoryShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel;
    private readonly Vehicle _vehicle;
    private readonly Customer _customer;
    private readonly Booking _booking;
    private readonly Rent _rent;

    public RentRepositoryShould()
    {
        _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _customer = Customer.Create(Guid.NewGuid());
        _booking = Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
        _rent = Rent.Create(_booking, _customer, _vehicle, _vehicleModel, TimeProvider.System);
    }

    [Fact]
    public async Task Add()
    {
        // Arrange
        await AddNeededAggregates();
        var (uow, repository) = Build(Context);

        // Act
        await repository.Add(_rent);
        await uow.Commit();

        // Assert
        _rent.ClearDomainEvents();
        var d = Context.Rents.First();
        await AssertEquivalentWithRentFromDb(_rent);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        await AddRent();
        var (uow, repository) = Build(Context);
        var rent = await repository.GetById(_rent.Id);
        rent!.Complete(TimeProvider.System);

        // Act
        repository.Update(rent);
        await uow.Commit();

        // Assert
        _rent.ClearDomainEvents();
        await AssertEquivalentWithRentFromDb(rent);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        await AddRent();
        var (_, repository) = Build(Context);

        // Act
        var actual = await repository.GetById(_rent.Id);

        // Assert
        _rent.ClearDomainEvents();
        Assert.Equivalent(_rent, actual);
    }

    private async Task AddRent()
    {
        await AddNeededAggregates();

        Context.Attach(_rent.Status);
        await Context.Rents.AddAsync(_rent);
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

        await Context.Bookings.AddAsync(_booking);
        await Context.SaveChangesAsync();
    }

    private (Infrastructure.Adapters.Postgres.UnitOfWork, RentRepository) Build(DataContext context)
    {
        return (new Infrastructure.Adapters.Postgres.UnitOfWork(context), new RentRepository(context));
    }

    private async Task AssertEquivalentWithRentFromDb(Rent expected)
    {
        var rentFromDb = await Context.Rents
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == _rent.Id);
        Assert.Equivalent(expected, rentFromDb);
    }
}