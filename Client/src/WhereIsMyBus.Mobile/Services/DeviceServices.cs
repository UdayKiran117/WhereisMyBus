using WhereIsMyBus.Mobile.Core.Interfaces;
namespace WhereIsMyBus.Mobile.Services;
public sealed class ConnectivityService : IConnectivityService
{
    public bool IsConnected => Connectivity.Current.NetworkAccess==NetworkAccess.Internet;
}
public sealed class UserPreferences : IUserPreferences
{
    public string Language { get=>Preferences.Default.Get("language","en");set=>Preferences.Default.Set("language",value); }
}
