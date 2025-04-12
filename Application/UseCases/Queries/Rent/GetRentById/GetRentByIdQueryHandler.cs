using Dapper;
using Domain.RentAggregate;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;
using Npgsql;

namespace Application.UseCases.Queries.Rent.GetRentById;

public class GetRentByIdQueryHandler(
    NpgsqlDataSource dataSource) : IRequestHandler<GetRentByIdQuery, Result<GetRentByIdQueryResponse>>
{
    public async Task<Result<GetRentByIdQueryResponse>> Handle(
        GetRentByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        var rent = await connection.QuerySingleOrDefaultAsync<RentDapperModel>(Sql, new { RentId = query.RentId });
        if (rent == null) return Result.Fail(new NotFound("Rent not found"));

        return new GetRentByIdQueryResponse(
            rent.RentId,
            Status.FromId(rent.StatusId),
            rent.BookingId,
            rent.CustomerId,
            rent.VehicleId,
            rent.Start,
            rent.End,
            rent.ActualAmount);
    }

    private record RentDapperModel(
        Guid RentId,
        int StatusId,
        Guid BookingId,
        Guid CustomerId,
        Guid VehicleId,
        DateTime Start,
        DateTime? End,
        decimal? ActualAmount);

    private const string Sql =
        """
        SELECT id AS RentId,
               status_id AS StatusId,
               booking_id AS BookingId,
               customer_id AS CustomerId,
               vehicle_id AS VehicleId,
               start AS Start,
               "end" AS "End",
               actual_amount AS ActualAmount
        FROM rent
        WHERE id = @RentId
        """;
}