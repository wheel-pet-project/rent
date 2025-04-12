using Application.UseCases.Commands.VehicleModel.AddVehicleModel;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class ModelCreatedConsumerEvent(
    Guid eventId,
    Guid modelId,
    double pricePerMinute,
    double pricePerHour,
    double pricePerDay) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid ModelId { get; } = modelId;
    public double PricePerMinute { get; } = pricePerMinute;
    public double PricePerHour { get; } = pricePerHour;
    public double PricePerDay { get; } = pricePerDay;

    public IRequest<Result> ToCommand()
    {
        return new AddVehicleModelCommand(ModelId, PricePerMinute, PricePerHour, PricePerDay);
    }
}