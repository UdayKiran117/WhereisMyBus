using Dapper;
using Microsoft.Extensions.Options;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Contracts.Responses;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Infrastructure.Configuration;
using WhereIsMyBus.Infrastructure.Persistence;
namespace WhereIsMyBus.Infrastructure.Repositories;

public sealed class CatalogRepository(
    MySqlConnectionFactory factory,
    IOptions<DatabaseOptions> options) : ICatalogRepository
{
    private const string BusColumns = """
        b.id AS Id,
        b.registration_no AS RegistrationNumber,
        b.operator_name AS OperatorName,
        b.display_name AS DisplayName,
        b.active AS IsActive
        """;

    private const string RouteSql = """
        SELECT
            r.id AS Id,
            r.name AS Name,
            r.origin_location_id AS SourceId,
            r.destination_location_id AS DestinationId,
            s.name AS SourceName,
            d.name AS DestinationName
        FROM routes r
        JOIN locations s ON s.id = r.origin_location_id
        JOIN locations d ON d.id = r.destination_location_id
        WHERE r.active = 1
        """;

    private CommandDefinition Command(
        string sql,
        object? args,
        CancellationToken ct) =>
        new(
            sql,
            args,
            commandTimeout: options.Value.CommandTimeoutSeconds,
            cancellationToken: ct);

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        object? args,
        CancellationToken ct)
    {
        await using var db = await factory.OpenAsync(ct);

        return (await db.QueryAsync<T>(
            Command(sql, args, ct))).AsList();
    }

    public Task<IReadOnlyList<Bus>> ListBusesAsync(
        int limit,
        CancellationToken ct) =>
        QueryAsync<Bus>(
            $"""
            SELECT {BusColumns}
            FROM buses b
            WHERE b.active = 1
            ORDER BY b.registration_no
            LIMIT @Limit
            """,
            new { Limit = limit },
            ct);

    public async Task<Bus?> GetBusAsync(
        long id,
        CancellationToken ct) =>
        (await QueryAsync<Bus>(
            $"""
            SELECT {BusColumns}
            FROM buses b
            WHERE b.id = @Id
              AND b.active = 1
            """,
            new { Id = id },
            ct)).SingleOrDefault();

    public Task<IReadOnlyList<Bus>> SearchBusesAsync(
        string normalizedRegistration,
        CancellationToken ct) =>
        QueryAsync<Bus>(
            $"""
            SELECT {BusColumns}
            FROM buses b
            WHERE b.registration_no = @Number
              AND b.active = 1
            """,
            new { Number = normalizedRegistration },
            ct);

    public Task<IReadOnlyList<Bus>> GetBusesByRouteAsync(
        long fromId,
        long toId,
        CancellationToken ct) =>
        QueryAsync<Bus>(
            $"""
            SELECT DISTINCT {BusColumns}
            FROM buses b
            JOIN bus_routes a
                ON a.bus_id = b.id
               AND a.active = 1
            JOIN routes r
                ON r.id = a.route_id
               AND r.active = 1
            JOIN route_stops f
                ON f.route_id = r.id
               AND f.location_id = @FromId
            JOIN route_stops t
                ON t.route_id = r.id
               AND t.location_id = @ToId
            WHERE b.active = 1
              AND f.stop_sequence < t.stop_sequence
            ORDER BY b.registration_no
            LIMIT 100
            """,
            new { FromId = fromId, ToId = toId },
            ct);

    public Task<IReadOnlyList<LocationResponse>> SearchLocationsAsync(
        string query,
        CancellationToken ct) =>
        QueryAsync<LocationResponse>(
            """
            SELECT
                id AS Id,
                name AS Name
            FROM locations
            WHERE active = 1
              AND (@Query = '' OR LOCATE(@Query, name) > 0)
            ORDER BY name
            LIMIT 50
            """,
            new { Query = query },
            ct);

    public Task<IReadOnlyList<RouteResponse>> ListRoutesAsync(
        CancellationToken ct) =>
        QueryAsync<RouteResponse>(
            RouteSql + " ORDER BY r.name LIMIT 100",
            null,
            ct);

    public async Task<RouteResponse?> GetRouteAsync(
        long id,
        CancellationToken ct) =>
        (await QueryAsync<RouteResponse>(
            RouteSql + " AND r.id = @Id",
            new { Id = id },
            ct)).SingleOrDefault();


}