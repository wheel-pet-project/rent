using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.ChangeVehicleModelTariff;

public class ChangeVehicleModelTariffHandler(
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeVehicleModelTariffCommand, Result>
{
    public async Task<Result> Handle(ChangeVehicleModelTariffCommand command, CancellationToken _)
    {
        var vehicleModel = await vehicleModelRepository.GetById(command.VehicleModelId);
        if (vehicleModel == null)
            throw new DataConsistencyViolationException(
                $"Vehicle model with id: {command.VehicleModelId} not found for changing tariff");

        var potentialTariff = Tariff.Create(
            (decimal)command.PricePerMinute,
            (decimal)command.PricePerHour,
            (decimal)command.PricePerDay);

        vehicleModel.ChangeTariff(potentialTariff);

        vehicleModelRepository.Update(vehicleModel);

        return await unitOfWork.Commit();
    }
}