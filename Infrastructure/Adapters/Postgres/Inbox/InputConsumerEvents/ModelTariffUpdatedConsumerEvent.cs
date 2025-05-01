using Application.UseCases.Commands.VehicleModel.ChangeVehicleModelTariff;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class ModelTariffUpdatedConsumerEvent(
    Guid eventId,
    Guid modelId,
    double pricePerMinute,
    double pricePerHour,
    double pricePerDay) : IConvertibleToCommand
{
    public Guid EventId { get; } = eventId;
    public Guid ModelId { get; } = modelId;
    public double PricePerMinute { get; } = pricePerMinute;
    public double PricePerHour { get; } = pricePerHour;
    public double PricePerDay { get; } = pricePerDay;

    public IRequest<Result> ToCommand()
    {
        return new ChangeVehicleModelTariffCommand(ModelId, PricePerMinute, PricePerHour, PricePerDay);
    }
}