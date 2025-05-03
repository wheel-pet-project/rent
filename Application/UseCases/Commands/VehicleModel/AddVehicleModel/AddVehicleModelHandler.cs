using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Vehicle.AddVehicle;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.AddVehicleModel;

public class AddVehicleModelHandler(
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddVehicleModelCommand, Result>
{
    public async Task<Result> Handle(AddVehicleModelCommand command, CancellationToken cancellationToken)
    {
        await ThrowIfVehicleModelExist(command);

        var tariff = Tariff.Create(
            (decimal)command.PricePerMinute,
            (decimal)command.PricePerHour,
            (decimal)command.PricePerDay);

        var vehicleModel = Domain.VehicleModelAggregate.VehicleModel.Create(command.VehicleModelId, tariff);

        await vehicleModelRepository.Add(vehicleModel);

        return await unitOfWork.Commit();
    }

    private async Task ThrowIfVehicleModelExist(AddVehicleModelCommand command)
    {
        if (await vehicleModelRepository.GetById(command.VehicleModelId) != null)
            throw new AlreadyHaveThisStateException("Vehicle model already exists");
    }
}