using WhereIsMyBus.Mobile.Core.Models;
namespace WhereIsMyBus.Mobile.Core.Interfaces;
public interface IBusApiClient
{
    Task<IReadOnlyList<BusModel>> SearchAsync(string registration,CancellationToken ct);
    Task<IReadOnlyList<BusModel>> ByRouteAsync(long fromId,long toId,CancellationToken ct);
    Task<IReadOnlyList<LocationModel>> LocationsAsync(CancellationToken ct);
}
public interface IConnectivityService { bool IsConnected { get; } }
public interface IUserPreferences { string Language { get; set; } }
public interface ITextService
{
    string Language { get; }
    string this[string key] { get; }
    void SetLanguage(string language);
}
