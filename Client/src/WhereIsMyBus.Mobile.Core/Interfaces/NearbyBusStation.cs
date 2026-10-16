namespace WhereIsMyBus.Mobile.Core.Interfaces;

public sealed record NearbyBusStation(
    long Id,
    string Name,
    double Latitude,
    double Longitude,
    double DistanceMeters)
{
    public string DistanceText => DistanceMeters < 1000
        ? $"{DistanceMeters:0} m away"
        : $"{DistanceMeters / 1000:0.0} km away";
}

public interface INearbyBusStationService
{
    Task<IReadOnlyList<NearbyBusStation>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken ct = default);
}