using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.StartRent;

public record StartRentCommand(
    Guid CustomerId,
    Guid BookingId,
    Guid VehicleId) : IRequest<Result<StartRentResponse>>;