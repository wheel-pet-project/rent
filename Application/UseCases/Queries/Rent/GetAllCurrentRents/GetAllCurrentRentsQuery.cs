using FluentResults;
using MediatR;

namespace Application.UseCases.Queries.Rent.GetAllCurrentRents;

public record GetAllCurrentRentsQuery(
    int? Page,
    int? PageSize) : IRequest<Result<GetAllCurrentRentsQueryResponse>>;