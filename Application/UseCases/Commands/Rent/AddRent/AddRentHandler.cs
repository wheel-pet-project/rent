using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Rent.AddRent;

public class AddRentHandler(
    IRentRepository rentRepository,
    ICustomerRepository customerRepository, 
    IBookingRepository bookingRepository,
    IVehicleRepository vehicleRepository,
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<AddRentCommand, Result<AddRentResponse>>
{
    public async Task<Result<AddRentResponse>> Handle(AddRentCommand command, CancellationToken cancellationToken)
    {
        Domain.BookingAggregate.Booking? booking = null;
        Domain.CustomerAggregate.Customer? customer = null;
        Domain.VehicleAggregate.Vehicle? vehicle = null;
        
        var tasks = new List<Task>();
        tasks.Add(GetBooking());
        tasks.Add(GetCustomer());
        tasks.Add(GetVehicle());
        
        await Task.WhenAll(tasks);
        
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
            ? new AddRentResponse(rent.Id)
            : commitResult;
        
        
        async Task GetBooking() => booking = await bookingRepository.GetById(command.BookingId);

        async Task GetCustomer() => customer = await customerRepository.GetById(command.CustomerId);

        async Task GetVehicle() => vehicle = await vehicleRepository.GetById(command.VehicleId);
    }
}