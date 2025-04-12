using Dapper;
using FluentResults;
using MediatR;
using Npgsql;

namespace Application.UseCases.Queries.Rent.GetAllCurrentRents;

public class GetAllCurrentRentsQueryHandler(
    NpgsqlDataSource dataSource) : IRequestHandler<GetAllCurrentRentsQuery, Result<GetAllCurrentRentsQueryResponse>>
{
    public async Task<Result<GetAllCurrentRentsQueryResponse>> Handle(
        GetAllCurrentRentsQuery query,
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        var rents = (await connection.QueryAsync<RentDapperModel>(Sql, new
        {
            Limit = query.PageSize,
            Offset = CalculateOffset(query.Page, query.PageSize)
        })).AsList();

        return new GetAllCurrentRentsQueryResponse(rents
            .Select(x => new RentShortModel(x.RentId, x.VehicleId, x.CustomerId, x.Start))
            .ToList());
    }

    private int CalculateOffset(int page, int pageSize)
    {
        return page < 1
            ? 1
            : (page - 1) * pageSize;
    }

    private record RentDapperModel(Guid RentId, Guid VehicleId, Guid CustomerId, DateTime Start);

    private const string Sql =
        """
        SELECT id AS RentId, 
               vehicle_id AS VehicleId, 
               customer_id AS CustomerId, 
               start AS Start 
        FROM rent
        WHERE "end" IS NULL
        LIMIT @Limit 
        OFFSET @Offset
        """;
}