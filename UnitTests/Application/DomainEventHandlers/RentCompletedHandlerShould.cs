using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.CustomerAggregate;
using Domain.RentAggregate.DomainEvents;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(RentCompletedHandler))]
public class RentCompletedHandlerShould
{
    private readonly RentCompletedDomainEvent _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        Guid.NewGuid(), 100);
    
    private readonly Customer _customer = Customer.Create(Guid.NewGuid());
    
    private readonly Mock<IMessageBus> _messageBusMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly RentCompletedHandler _handler;
    
    public RentCompletedHandlerShould()
    {
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new RentCompletedHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object,
            _messageBusMock.Object);
    }

    [Fact]
    public async Task ThrowDataConsistencyExceptionIfCustomerNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as Customer);

        // Act
        async Task Act() => await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }
    
    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange

        // Act
        await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<RentCompletedDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task ThrowExceptionInCommitErrorIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new DataConsistencyViolationException())));

        // Act
        async Task Act() => await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }
}