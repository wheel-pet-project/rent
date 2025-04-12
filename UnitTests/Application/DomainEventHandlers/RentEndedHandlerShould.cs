using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Domain.RentAggregate.DomainEvents;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(RentEndedHandler))]
public class RentEndedHandlerShould
{
    private readonly RentCompletedDomainEvent _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        100);

    private readonly Mock<IMessageBus> _messageBusMock = new();

    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange
        var handler = new RentEndedHandler(_messageBusMock.Object);

        // Act
        await handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<RentCompletedDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}