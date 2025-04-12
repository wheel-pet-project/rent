using Domain.RentAggregate;

namespace Application.UseCases.Queries.Rent.GetRentById;

public record GetRentByIdQueryResponse(
    Guid RentId,
    Status Status,
    Guid BookingId,
    Guid CustomerId,
    Guid VehicleId,
    DateTime Start,
    DateTime? End,
    decimal? ActualAmount);