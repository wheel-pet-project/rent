using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.AddCompletedBooking;

public record AddCompletedBookingCommand(
    Guid BookingId,
    Guid VehicleId,
    Guid CustomerId) : IRequest<Result>;