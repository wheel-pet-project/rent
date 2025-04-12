using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
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
    public async Task<Result<StartRentResponse>> Handle(StartRentCommand command, CancellationToken _)
    {
        var (booking, customer, vehicle) = await GetAggregatesInParallel(
            command.BookingId, 
            command.CustomerId, 
            command.VehicleId);
        
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
    
    private async Task<(
        Domain.BookingAggregate.Booking?, 
        Domain.CustomerAggregate.Customer?, 
        Domain.VehicleAggregate.Vehicle?)> GetAggregatesInParallel(Guid bookingId, Guid customerId, Guid vehicleId)
    {
        Domain.BookingAggregate.Booking? booking = null;
        Domain.CustomerAggregate.Customer? customer = null;
        Domain.VehicleAggregate.Vehicle? vehicle = null;

        await Task.WhenAll(GetBooking(), GetCustomer(), GetVehicle());
            
        return (booking, customer, vehicle);
        

        async Task GetBooking() => booking = await bookingRepository.GetById(bookingId);

        async Task GetCustomer() => customer = await customerRepository.GetById(customerId);

        async Task GetVehicle() => vehicle = await vehicleRepository.GetById(vehicleId);
    }
}