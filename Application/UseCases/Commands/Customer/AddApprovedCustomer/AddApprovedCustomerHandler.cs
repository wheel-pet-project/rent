using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddApprovedCustomer;

public class AddApprovedCustomerHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddApprovedCustomerCommand, Result>
{
    public async Task<Result> Handle(AddApprovedCustomerCommand command, CancellationToken cancellationToken)
    {
        var existingCustomer = await customerRepository.GetById(command.CustomerId);
        if (existingCustomer != null)
            throw new AlreadyHaveThisStateException(
                $"Customer with id: {command.CustomerId} already exists");

        var customer = Domain.CustomerAggregate.Customer.Create(command.CustomerId);

        await customerRepository.Add(customer);

        return await unitOfWork.Commit();
    }
}