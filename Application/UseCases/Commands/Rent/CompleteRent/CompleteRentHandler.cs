using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.CompleteRent;

public class CompleteRentHandler(
    IRentRepository rentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CompleteRentCommand, Result<CompleteRentResponse>>
{
    public async Task<Result<CompleteRentResponse>> Handle(
        CompleteRentCommand command,
        CancellationToken cancellationToken)
    {
        var rent = await rentRepository.GetById(command.RentId);
        if (rent == null) return Result.Fail(new NotFound("Rent not found"));

        var actualAmount = rent.Complete(timeProvider);

        rentRepository.Update(rent);

        var commitResult = await unitOfWork.Commit();

        return commitResult.IsSuccess
            ? new CompleteRentResponse(actualAmount)
            : commitResult;
    }
}