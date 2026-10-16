namespace WhereIsMyBus.Mobile.Core.Interfaces;

public sealed record DashboardBus(long Id, string RegistrationNumber);

public sealed record DashboardDestination(long Id, string Name);

public sealed record DriverCheckInRequest(long BusId, long DestinationId);

public sealed record DriverCheckInResponse(bool Success, string Message);

public interface IDriverDashboardService
{
    // Return buses assigned to the authenticated driver.
    Task<IReadOnlyList<DashboardBus>> GetAssignedBusesAsync(
        CancellationToken ct = default);

    // Return valid destinations for the selected bus.
    Task<IReadOnlyList<DashboardDestination>> GetDestinationsAsync(
        long busId,
        CancellationToken ct = default);

    Task<DriverCheckInResponse> CheckInAsync(
        DriverCheckInRequest request,
        CancellationToken ct = default);
}