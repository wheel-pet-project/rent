using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Vehicle.AddVehicle;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Vehicle;

[TestSubject(typeof(AddVehicleHandler))]
public class AddVehicleHandlerShould
{
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel =
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 300.0M, 4000.0M));

    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddVehicleCommand _command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly AddVehicleHandler _handler;

    public AddVehicleHandlerShould()
    {
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicleModel);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new AddVehicleHandler(_vehicleModelRepositoryMock.Object, _vehicleRepositoryMock.Object,
            _unitOfWorkMock.Object);
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
    public async Task ThrowAlreadyHaveThisStateExceptionIfVehicleAlreadyExist()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(
                global::Domain.VehicleAggregate.Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel));

        // Act
        async Task Act()
        {
            await _handler.Handle(_command, TestContext.Current.CancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<AlreadyHaveThisStateException>(Act);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfVehicleModelNotFound()
    {
        // Arrange
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleModelAggregate.VehicleModel);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
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