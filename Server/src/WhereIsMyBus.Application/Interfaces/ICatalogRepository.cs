using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Contracts.Responses;
namespace WhereIsMyBus.Application.Interfaces;
public interface ICatalogRepository
{
    Task<IReadOnlyList<Bus>> ListBusesAsync(int limit, CancellationToken ct);
    Task<Bus?> GetBusAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<Bus>> SearchBusesAsync(string normalizedRegistration, CancellationToken ct);
    Task<IReadOnlyList<Bus>> GetBusesByRouteAsync(long fromId, long toId, CancellationToken ct);
    Task<IReadOnlyList<LocationResponse>> SearchLocationsAsync(string query, CancellationToken ct);
    Task<IReadOnlyList<RouteResponse>> ListRoutesAsync(CancellationToken ct);
    Task<RouteResponse?> GetRouteAsync(long id, CancellationToken ct);
}
