using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;

namespace Application.UseCases.Queries.Rent.GetCurrentAmountRent;

public class GetCurrentAmountRentQueryHandler(
    IRentRepository rentRepository,
    TimeProvider timeProvider)
    : IRequestHandler<GetCurrentAmountRentQuery, Result<GetCurrentAmountRentQueryResponse>>
{
    public async Task<Result<GetCurrentAmountRentQueryResponse>> Handle(
        GetCurrentAmountRentQuery query,
        CancellationToken cancellationToken)
    {
        var rent = await rentRepository.GetById(query.RentId);
        if (rent == null) return Result.Fail(new NotFound("Rent not found"));

        var currentAmount = rent.GetCurrentAmount(timeProvider);

        return new GetCurrentAmountRentQueryResponse(rent.Id, currentAmount);
    }
}