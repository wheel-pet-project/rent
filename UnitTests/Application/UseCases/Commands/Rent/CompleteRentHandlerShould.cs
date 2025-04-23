using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Rent.CompleteRent;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Rent;

[TestSubject(typeof(CompleteRentHandler))]
public class CompleteRentHandlerShould
{
    private readonly global::Domain.CustomerAggregate.Customer _customer;
    private readonly global::Domain.BookingAggregate.Booking _booking;
    private readonly global::Domain.VehicleAggregate.Vehicle _vehicle;
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel;
    private readonly global::Domain.RentAggregate.Rent _rent;
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private readonly Mock<IRentRepository> _rentRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly CompleteRentCommand _command = new(Guid.NewGuid());

    private readonly CompleteRentHandler _handler;

    public CompleteRentHandlerShould()
    {
        _customer = global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid());
        _vehicleModel = global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(),
            Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = global::Domain.VehicleAggregate.Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _booking = global::Domain.BookingAggregate.Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);
        _rent = global::Domain.RentAggregate.Rent.Create(_booking, _customer, _vehicle, _vehicleModel, _timeProvider);

        _rentRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_rent);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new CompleteRentHandler(_rentRepositoryMock.Object, _unitOfWorkMock.Object, _timeProvider);
    }

    [Fact]
    public async Task ReturnSuccessAndActualAmount()
    {
        // Arrange

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(actual);
        Assert.True(actual.IsSuccess);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfRentNotFound()
    {
        // Arrange
        _rentRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.RentAggregate.Rent);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ReturnCommitErrorIfCommitFailed()
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