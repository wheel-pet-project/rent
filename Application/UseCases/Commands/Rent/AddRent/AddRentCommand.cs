using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.AddRent;

public record AddRentCommand(
    Guid CustomerId, 
    Guid BookingId, 
    Guid VehicleId) : IRequest<Result<AddRentResponse>>;