using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.AddCompletedBooking;

public class AddCompletedBookingHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCompletedBookingCommand, Result>
{
    public async Task<Result> Handle(AddCompletedBookingCommand command, CancellationToken _)
    {
        await ThrowIfBookingExist(command);

        var existingBooking = await bookingRepository.GetById(command.BookingId);
        if (existingBooking != null)
            throw new AlreadyHaveThisStateException(
                $"Booking with id: {command.BookingId} already exists");

        var booking = Domain.BookingAggregate.Booking.Create(command.BookingId, command.VehicleId, command.CustomerId);

        await bookingRepository.Add(booking);

        return await unitOfWork.Commit();
    }

    private async Task ThrowIfBookingExist(AddCompletedBookingCommand command)
    {
        if (await bookingRepository.GetById(command.BookingId) != null)
            throw new AlreadyHaveThisStateException("Booking already exists");
    }
}