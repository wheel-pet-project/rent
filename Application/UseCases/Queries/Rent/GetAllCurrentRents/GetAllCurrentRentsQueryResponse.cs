namespace Application.UseCases.Queries.Rent.GetAllCurrentRents;

public record GetAllCurrentRentsQueryResponse(List<RentShortModel> Rents);

public record RentShortModel(Guid RentId, Guid VehicleId, Guid CustomerId, DateTime Start);