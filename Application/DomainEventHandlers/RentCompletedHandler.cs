using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.RentAggregate.DomainEvents;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using MediatR;

namespace Application.DomainEventHandlers;

public class RentCompletedHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IMessageBus messageBus) : INotificationHandler<RentCompletedDomainEvent>
{
    public async Task Handle(RentCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(domainEvent.CustomerId);
        if (customer == null)
            throw new DataConsistencyViolationException(
                $"Customer with id {domainEvent.CustomerId} not found");

        await messageBus.Publish(domainEvent, cancellationToken);

        customer.AddRent();

        if (customer.Level.IsNeededChange(customer.Points)) customer.ChangeToOneLevel();

        customerRepository.Update(customer);

        var commitResult = await unitOfWork.Commit();
        if (commitResult.IsFailed) throw ((CommitFail)commitResult.Errors[0]).Exception;
    }
}