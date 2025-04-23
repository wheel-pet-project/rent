using Domain.CustomerAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Repositories;

[TestSubject(typeof(CustomerRepository))]
public class CustomerRepositoryShould : IntegrationTestBase
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid());
    
    [Fact]
    public async Task Add()
    {
        // Arrange
        var (uow, repository) = Build(Context);

        // Act
        await repository.Add(_customer);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithCustomerFromDb(_customer);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        await AddCustomer();
        var (uow, repository) = Build(Context);
        var customer = await repository.GetById(_customer.Id);
        customer!.AddRent();

        // Act
        repository.Update(customer);
        await uow.Commit();

        // Assert
        await AssertEquivalentWithCustomerFromDb(customer);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        await AddCustomer();
        var (_, repository) = Build(Context);

        // Act
        var actual = await repository.GetById(_customer.Id);

        // Assert
        Assert.Equivalent(_customer, actual);
    }
    
    private async Task AddCustomer()
    {
        Context.Attach(_customer.Level);
        await Context.Customers.AddAsync(_customer);
        await Context.SaveChangesAsync();
    }
    
    private (Infrastructure.Adapters.Postgres.UnitOfWork, CustomerRepository) Build(DataContext context)
    {
        return (new Infrastructure.Adapters.Postgres.UnitOfWork(context), new CustomerRepository(context));
    }
    
    private async Task AssertEquivalentWithCustomerFromDb(Customer expected)
    {
        Context.ChangeTracker.Clear();
        var customerFromDb = await Context.Customers
            .Include(x => x.Level)
            .FirstOrDefaultAsync(x => x.Id == _customer.Id);
        Assert.Equivalent(expected, customerFromDb);
    }
}