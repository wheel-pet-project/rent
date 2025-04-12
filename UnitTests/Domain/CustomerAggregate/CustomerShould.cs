using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(Customer))]
public class CustomerShould
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly List<Category> _categories = [Category.Create(Category.BCategory)];

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = Customer.Create(_id);

        // Assert
        Assert.Equal(_id, actual.Id);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            Customer.Create(Guid.Empty);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }


    [Fact]
    public void ChangeToOneLevelChangeLevelToNext()
    {
        // Arrange
        var customer = Customer.Create(_id);
        for (var i = 0; i < 101; i++) customer.AddRent();

        // Act
        customer.ChangeToOneLevel();

        // Assert
        Assert.Equal(Level.Regular, customer.Level);
    }

    [Fact]
    public void AddRentIncrementRentQuantity()
    {
        // Arrange
        var customer = Customer.Create(_id);

        // Act
        customer.AddRent();

        // Assert
        Assert.Equal(1, customer.Rents);
    }
}