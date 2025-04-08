using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.VehicleModel.ChangeVehicleModelTariff;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.VehicleModel;

[TestSubject(typeof(ChangeVehicleModelTariffHandler))]
public class ChangeVehicleModelTariffHandlerShould
{
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel =
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Tariff.Create(10.0M, 300.0M, 4000.0M));
    
    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly ChangeVehicleModelTariffCommand _command = new(Guid.NewGuid(), 1.0, 3.0, 40.0);

    private readonly ChangeVehicleModelTariffHandler _handler;

    public ChangeVehicleModelTariffHandlerShould()
    {
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicleModel);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _handler = new(_vehicleModelRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ThrowDataConsistencyViolationExceptionIfVehicleModelIsNotFound()
    {
        // Arrange
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleModelAggregate.VehicleModel);

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
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