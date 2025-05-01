using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Rent.StartRent;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Rent;

[TestSubject(typeof(StartRentHandler))]
public class AddRentHandlerShould
{
    private readonly global::Domain.CustomerAggregate.Customer _customer;
    private readonly global::Domain.BookingAggregate.Booking _booking;
    private readonly global::Domain.VehicleAggregate.Vehicle _vehicle;
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel;

    private readonly Mock<IRentRepository> _rentRepositoryMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock = new();
    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private readonly StartRentCommand _command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly StartRentHandler _handler;

    public AddRentHandlerShould()
    {
        _customer = global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid());
        _vehicleModel = global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(),
            Tariff.Create(10.0M, 200.0M, 1000.0M));
        _vehicle = global::Domain.VehicleAggregate.Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        _booking = global::Domain.BookingAggregate.Booking.Create(Guid.NewGuid(), _vehicle.Id, _customer.Id);

        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_booking);
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicle);
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicleModel);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new StartRentHandler(_rentRepositoryMock.Object, _customerRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _vehicleRepositoryMock.Object, _vehicleModelRepositoryMock.Object, _unitOfWorkMock.Object, _timeProvider);
    }

    [Fact]
    public async Task ReturnSuccessAndRentId()
    {
        // Arrange

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.NotEqual(Guid.Empty, actual.Value.RentId);
    }

    [Fact]
    public async Task ThrowAlreadyHaveThisStateExceptionIfRentAlreadyExists()
    {
        // Arrange
        _rentRepositoryMock.Setup(x => x.GetByBookingId(It.IsAny<Guid>()))
            .ReturnsAsync(global::Domain.RentAggregate.Rent.Create(_booking, _customer, _vehicle, _vehicleModel,
                _timeProvider));

        // Act
        async Task Act()
        {
            await _handler.Handle(_command, TestContext.Current.CancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<AlreadyHaveThisStateException>(Act);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfBookingNotFound()
    {
        // Arrange
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.BookingAggregate.Booking);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfCustomerNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.CustomerAggregate.Customer);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfVehicleNotFound()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleAggregate.Vehicle);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ThrowDataConsistencyViolationExceptionIfVehicleModelNotFound()
    {
        // Arrange
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleModelAggregate.VehicleModel);

        // Act
        async Task Act()
        {
            await _handler.Handle(_command, TestContext.Current.CancellationToken);
        }

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
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}