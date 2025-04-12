namespace Application.UseCases.Queries.Rent.GetCurrentAmountRent;

public record GetCurrentAmountRentQueryResponse(
    Guid RentId,
    decimal CurrentAmount);