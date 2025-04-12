using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(RentStartedHandler))]
public class RentStartedHandlerShould
{
    private readonly RentStartedDomainEvent
        _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly Mock<IMessageBus> _messageBusMock = new();

    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange
        var handler = new RentStartedHandler(_messageBusMock.Object);

        // Act
        await handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<RentStartedDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}