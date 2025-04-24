using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(VehicleModelRepository))]
public class VehicleModelRepositoryShould : IntegrationTestBase
{
    private readonly VehicleModel _model = VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 300.0M, 600.0M));

    [Fact]
    public async Task Add()
    {
        // Arrange
        await AddVehicleModel();
        var (uow, repository) = Build(Context);

        // Act
        await repository.Add(_model);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithModelFromDb(_model);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        await AddVehicleModel();
        var (uow, repository) = Build(Context);
        var model = await repository.GetById(_model.Id);
        model!.ChangeTariff(Tariff.Create(20.0M, 400.0M, 700.0M));

        // Act
        repository.Update(model);
        await uow.Commit();
        
        // Assert
        await AssertEquivalentWithModelFromDb(model);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        await AddVehicleModel();
        var (_, repository) = Build(Context);

        // Act
        var actual = await repository.GetById(_model.Id);
        
        // Assert
        Assert.Equivalent(_model, actual);
    }
    
    private async Task AddVehicleModel()
    {
        await Context.VehicleModels.AddAsync(_model);
        await Context.SaveChangesAsync();
    }
    
    private (Infrastructure.Adapters.Postgres.UnitOfWork, VehicleModelRepository) Build(DataContext context)
    {
        return (new Infrastructure.Adapters.Postgres.UnitOfWork(context), new VehicleModelRepository(context));
    }
    
    private async Task AssertEquivalentWithModelFromDb(VehicleModel expected)
    {
        Context.ChangeTracker.Clear();
        var modelFromDb = await Context.VehicleModels.FirstOrDefaultAsync(x => x.Id == _model.Id);
        Assert.Equivalent(expected, modelFromDb);
    }
}