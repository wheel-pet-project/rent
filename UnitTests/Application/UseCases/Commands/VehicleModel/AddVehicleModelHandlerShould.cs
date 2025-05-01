using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.VehicleModel.AddVehicleModel;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.VehicleModel;

[TestSubject(typeof(AddVehicleModelHandler))]
public class AddVehicleModelHandlerShould
{
    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddVehicleModelCommand _command = new(Guid.NewGuid(), 10.0, 300.0, 4000.0);

    private readonly AddVehicleModelHandler _handler;

    public AddVehicleModelHandlerShould()
    {
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new AddVehicleModelHandler(_vehicleModelRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ThrowAlreadyHaveThisStateException()
    {
        // Arrange
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(),
                Tariff.Create(10.0M, 200.0M, 1000.0M)));

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<AlreadyHaveThisStateException>(Act);
    }
    
    [Fact]
    public async Task ReturnCommitFailErrorIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new Exception())));

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}