using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Contracts.Responses;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Domain.Validation;
namespace WhereIsMyBus.Application.Services;
public sealed class CatalogService(ICatalogRepository repository)
{
    private static BusResponse Map(Bus bus) => new(bus.Id, bus.RegistrationNumber, bus.OperatorName, bus.DisplayName);
    public async Task<IReadOnlyList<BusResponse>> ListAsync(int limit, CancellationToken ct = default)
    {
        if (limit is < 1 or > 100) throw new ArgumentException("Limit must be between 1 and 100.");
        return (await repository.ListBusesAsync(limit, ct)).Select(Map).ToArray();
    }
    public async Task<BusResponse?> GetAsync(long id, CancellationToken ct = default)
    {
        ValidateId(id);
        var bus = await repository.GetBusAsync(id, ct);
        return bus is null ? null : Map(bus);
    }
    public async Task<IReadOnlyList<BusResponse>> SearchAsync(string? number, CancellationToken ct = default) =>
        (await repository.SearchBusesAsync(RegistrationNumber.Normalize(number), ct)).Select(Map).ToArray();
    public async Task<IReadOnlyList<BusResponse>> ByRouteAsync(long fromId, long toId, CancellationToken ct = default)
    {
        ValidateId(fromId); ValidateId(toId);
        if (fromId == toId) throw new ArgumentException("Choose different From and To locations.");
        return (await repository.GetBusesByRouteAsync(fromId, toId, ct)).Select(Map).ToArray();
    }
    public Task<IReadOnlyList<LocationResponse>> LocationsAsync(string? query, CancellationToken ct = default)
    {
        query = query?.Trim() ?? "";
        if (query.Length > 80) throw new ArgumentException("Location search must be at most 80 characters.");
        return repository.SearchLocationsAsync(query, ct);
    }
    public Task<IReadOnlyList<RouteResponse>> RoutesAsync(CancellationToken ct = default) => repository.ListRoutesAsync(ct);
    public Task<RouteResponse?> RouteAsync(long id, CancellationToken ct = default)
    {
        ValidateId(id); return repository.GetRouteAsync(id, ct);
    }
    private static void ValidateId(long id)
    {
        if (id <= 0) throw new ArgumentException("Select a valid record.");
    }
}
