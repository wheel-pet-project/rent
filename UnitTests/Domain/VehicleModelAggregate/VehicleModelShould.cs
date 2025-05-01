using Domain.SharedKernel.Exceptions.PublicExceptions;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.VehicleModelAggregate;

[TestSubject(typeof(VehicleModel))]
public class VehicleModelShould
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly Category _category = Category.Create(Category.BCategory);
    private readonly Tariff _tariff = Tariff.Create(10.0M, 500.0M, 2000.0M);

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = VehicleModel.Create(_id, _tariff);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_id, actual.Id);
        Assert.Equal(_tariff, actual.Tariff);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            VehicleModel.Create(Guid.Empty, _tariff);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }


    [Fact]
    public void ThrowValueIsRequiredExceptionIfTariffIsNull()
    {
        // Arrange

        // Act
        void Act()
        {
            VehicleModel.Create(_id, null!);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ChangeTariffThrowValueIsRequiredExceptionITariffIsNull()
    {
        // Arrange
        var vehicleModel = VehicleModel.Create(_id, _tariff);

        // Act
        void Act()
        {
            vehicleModel.ChangeTariff(null!);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ChangeTariffSetNewTariff()
    {
        // Arrange
        var vehicleModel = VehicleModel.Create(_id, _tariff);
        var newTariff = Tariff.Create(20.0M, 700.0M, 3000.0M);

        // Act
        vehicleModel.ChangeTariff(newTariff);

        // Assert
        Assert.Equal(newTariff, vehicleModel.Tariff);
    }
}