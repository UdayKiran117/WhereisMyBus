using System.Globalization;
using System.Net.Http.Json;
using WhereIsMyBus.Mobile.Core.Interfaces;

namespace WhereIsMyBus.Mobile.Services;

public sealed class NearbyBusStationService(HttpClient client)
    : INearbyBusStationService
{
    public async Task<IReadOnlyList<NearbyBusStation>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken ct = default)
    {
        var url = FormattableString.Invariant(
    $"api/stations/nearby?latitude={latitude}&longitude={longitude}&radiusKm={radiusKm}");
        using var response = await client.GetAsync(url, ct);

        response.EnsureSuccessStatusCode();

        var stations = await response.Content.ReadFromJsonAsync<List<NearbyBusStation>>(cancellationToken: ct);

        return stations ?? [];
    }
}