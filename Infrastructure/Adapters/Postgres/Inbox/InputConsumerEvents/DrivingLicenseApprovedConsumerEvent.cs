using Application.UseCases.Commands.Customer.AddApprovedCustomer;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class DrivingLicenseApprovedConsumerEvent(
    Guid eventId,
    Guid customerId) : IConvertibleToCommand
{
    public Guid EventId { get; } = eventId;
    public Guid CustomerId { get; } = customerId;

    public IRequest<Result> ToCommand()
    {
        return new AddApprovedCustomerCommand(CustomerId);
    }
}