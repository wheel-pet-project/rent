using FluentResults;
using MediatR;

namespace Application.UseCases.Queries.Rent.GetCurrentAmountRent;

public record GetCurrentAmountRentQuery(Guid RentId) : IRequest<Result<GetCurrentAmountRentQueryResponse>>;