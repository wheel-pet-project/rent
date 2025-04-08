using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Customer.AddApprovedCustomer;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Customer;

[TestSubject(typeof(AddApprovedCustomerHandler))]
public class AddApprovedCustomerHandlerShould
{
    private readonly global::Domain.CustomerAggregate.Customer _customer =
        global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid());
    
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddApprovedCustomerCommand _command = new(Guid.NewGuid());
    
    private readonly AddApprovedCustomerHandler _handler;

    public AddApprovedCustomerHandlerShould()
    {
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.CustomerAggregate.Customer);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _handler = new(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task ReturnSuccess()
    {
        // Arrange

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
    }

    [Fact]
    public async Task ThrowAlreadyHaveThisStateExceptionIfCustomerAlreadyExists()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<AlreadyHaveThisStateException>(Act);
    }
    
    [Fact]
    public async Task ReturnCommitErrorIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new Exception())));

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);
        
        // Assert
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}