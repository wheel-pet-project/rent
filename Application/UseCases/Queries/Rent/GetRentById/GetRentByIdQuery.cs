using FluentResults;
using MediatR;

namespace Application.UseCases.Queries.Rent.GetRentById;

public record GetRentByIdQuery(Guid RentId) : IRequest<Result<GetRentByIdQueryResponse>>;