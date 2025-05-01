using Application.UseCases.Commands.Booking.AddCompletedBooking;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class BookingCompletedConsumerEvent(
    Guid eventId,
    Guid bookingId,
    Guid vehicleId,
    Guid customerId) : IConvertibleToCommand
{
    public Guid EventId { get; } = eventId;
    public Guid BookingId { get; } = bookingId;
    public Guid VehicleId { get; } = vehicleId;
    public Guid CustomerId { get; } = customerId;

    public IRequest<Result> ToCommand()
    {
        return new AddCompletedBookingCommand(BookingId, VehicleId, CustomerId);
    }
}