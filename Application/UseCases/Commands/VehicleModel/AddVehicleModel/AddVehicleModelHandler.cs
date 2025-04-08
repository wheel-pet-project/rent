using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
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
        var tariff = Tariff.Create(
            (decimal)command.PricePerMinute, 
            (decimal)command.PricePerHour,
            (decimal)command.PricePerDay);
        
        var vehicleModel = Domain.VehicleModelAggregate.VehicleModel.Create(command.VehicleModelId, tariff);
        
        await vehicleModelRepository.Add(vehicleModel);
        
        return await unitOfWork.Commit();
    }
}