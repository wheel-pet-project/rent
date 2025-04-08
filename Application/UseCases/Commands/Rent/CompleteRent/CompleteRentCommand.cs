using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.CompleteRent;

public record CompleteRentCommand(Guid RentId) : IRequest<Result<CompleteRentResponse>>;