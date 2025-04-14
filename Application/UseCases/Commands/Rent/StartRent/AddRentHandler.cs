using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.StartRent;

public class StartRentHandler(
    IRentRepository rentRepository,
    ICustomerRepository customerRepository, 
    IBookingRepository bookingRepository,
    IVehicleRepository vehicleRepository,
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<StartRentCommand, Result<StartRentResponse>>
{
    public async Task<Result<StartRentResponse>> Handle(StartRentCommand command, CancellationToken cancellationToken)
    {
        if (await rentRepository.GetByBookingId(command.BookingId) != null)
            throw new AlreadyHaveThisStateException("Rent already exists");
        
        var booking = await bookingRepository.GetById(command.BookingId);
        var customer = await customerRepository.GetById(command.CustomerId);
        var vehicle = await vehicleRepository.GetById(command.VehicleId);
        
        if (booking == null) return Result.Fail(new NotFound("Booking not found"));
        if (customer == null) return Result.Fail(new NotFound("Customer not found"));
        if (vehicle == null) return Result.Fail(new NotFound("Vehicle not found"));

        var vehicleModel = await vehicleModelRepository.GetById(vehicle.VehicleModelId);
        if (vehicleModel == null) throw new DataConsistencyViolationException(
            $"Vehicle model with id: {vehicle.VehicleModelId} not found");
        
        var rent = Domain.RentAggregate.Rent.Create(booking, customer, vehicle, vehicleModel, timeProvider);

        await rentRepository.Add(rent);

        var commitResult = await unitOfWork.Commit();
        
        return commitResult.IsSuccess
            ? new StartRentResponse(rent.Id)
            : commitResult;
    }
}