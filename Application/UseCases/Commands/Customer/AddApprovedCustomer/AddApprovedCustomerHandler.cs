using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddApprovedCustomer;

public class AddApprovedCustomerHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddApprovedCustomerCommand, Result>
{
    public async Task<Result> Handle(AddApprovedCustomerCommand command, CancellationToken cancellationToken)
    {
        await ThrowIfCustomerExist(command);

        var customer = Domain.CustomerAggregate.Customer.Create(command.CustomerId);

        await customerRepository.Add(customer);

        return await unitOfWork.Commit();
    }

    private async Task ThrowIfCustomerExist(AddApprovedCustomerCommand command)
    {
        if (await customerRepository.GetById(command.CustomerId) != null)
            throw new AlreadyHaveThisStateException("Customer already exists");
    }
}