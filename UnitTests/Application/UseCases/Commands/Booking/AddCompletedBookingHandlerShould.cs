using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Booking.AddCompletedBooking;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Booking;

[TestSubject(typeof(AddCompletedBookingHandler))]
public class AddCompletedBookingHandlerShould
{
    private readonly global::Domain.BookingAggregate.Booking _booking =
        global::Domain.BookingAggregate.Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddCompletedBookingCommand _command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly AddCompletedBookingHandler _handler;

    public AddCompletedBookingHandlerShould()
    {
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.BookingAggregate.Booking);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new AddCompletedBookingHandler(_bookingRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ThrowAlreadyHaveThisStateExceptionIfBookingAlreadyExists()
    {
        // Arrange
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_booking);

        // Act
        async Task Act()
        {
            await _handler.Handle(_command, TestContext.Current.CancellationToken);
        }

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