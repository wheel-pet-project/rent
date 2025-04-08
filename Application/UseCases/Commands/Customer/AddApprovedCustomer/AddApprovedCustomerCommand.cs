using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddApprovedCustomer;

public record AddApprovedCustomerCommand(Guid CustomerId) : IRequest<Result>;