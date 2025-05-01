using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.VehicleAggregate.DomainEvents;

[TestSubject(typeof(VehicleAddedDomainEventShould))]
public class VehicleAddedDomainEventShould
{
    private readonly Guid _sagaId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new global::Domain.VehicleAggregate.DomainEvents.VehicleAddedDomainEvent(_sagaId, _vehicleId);

        // Assert
        Assert.Equal(_vehicleId, actual.VehicleId);
    }
    
    [Fact]
    public void ThrowArgumentExceptionIfSagaIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new global::Domain.VehicleAggregate.DomainEvents.VehicleAddedDomainEvent(Guid.Empty, _vehicleId);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ThrowArgumentExceptionIfVehicleIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new global::Domain.VehicleAggregate.DomainEvents.VehicleAddedDomainEvent(_sagaId, Guid.Empty);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}