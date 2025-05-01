using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(VehicleRepository))]
public class VehicleRepositoryShould : IntegrationTestBase
{
    private readonly VehicleModel _model;
    private readonly Vehicle _vehicle;

    public VehicleRepositoryShould()
    {
        _model = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _model);
    }

    [Fact]
    public async Task Add()
    {
        // Arrange
        await AddNeededVehicleModel();
        var (uow, repository) = Build(Context);

        // Act
        await repository.Add(_vehicle);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithVehicleFromDb(_vehicle);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        await AddVehicle();
        var (uow, repository) = Build(Context);
        var vehicle = await repository.GetById(_vehicle.Id);
        vehicle!.Delete();

        // Act
        repository.Update(vehicle);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithVehicleFromDb(_vehicle);
    }

    private async Task AddVehicle()
    {
        await AddNeededVehicleModel();

        await Context.Vehicles.AddAsync(_vehicle);
        await Context.SaveChangesAsync();
    }

    private async Task AddNeededVehicleModel()
    {
        await Context.VehicleModels.AddAsync(_model);
        await Context.SaveChangesAsync();
    }

    private (Infrastructure.Adapters.Postgres.UnitOfWork, VehicleRepository) Build(DataContext context)
    {
        return (new Infrastructure.Adapters.Postgres.UnitOfWork(context), new VehicleRepository(context));
    }

    private async Task AssertEquivalentWithVehicleFromDb(Vehicle expected)
    {
        var vehicleFromDb = await Context.Vehicles.FirstOrDefaultAsync(x => x.Id == _vehicle.Id);
        Assert.Equivalent(expected, vehicleFromDb);
    }
}