using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.ChangeVehicleModelTariff;

public record ChangeVehicleModelTariffCommand(
    Guid VehicleModelId,
    double PricePerMinute,
    double PricePerHour,
    double PricePerDay) : IRequest<Result>;