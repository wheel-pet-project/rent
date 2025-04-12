using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.AddVehicleModel;

public record AddVehicleModelCommand(
    Guid VehicleModelId,
    double PricePerMinute,
    double PricePerHour,
    double PricePerDay) : IRequest<Result>;