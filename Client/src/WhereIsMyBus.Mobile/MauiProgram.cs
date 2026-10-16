using Microsoft.Extensions.Logging;
using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Core.Services;
using WhereIsMyBus.Mobile.Core.ViewModels;
using WhereIsMyBus.Mobile.Services;
using WhereIsMyBus.Mobile.Views;
namespace WhereIsMyBus.Mobile;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder=MauiApp.CreateBuilder();builder.UseMauiApp<App>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<DriverPage>();
        builder.Services.AddTransient<AppShell>();

        builder.Services.AddSingleton<IConnectivityService,ConnectivityService>();
        builder.Services.AddSingleton<IUserPreferences,UserPreferences>();
        builder.Services.AddSingleton<ITextService,TextService>();
        builder.Services.AddHttpClient<INearbyBusStationService, NearbyBusStationService>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7081/");
    });
        builder.Services.AddHttpClient<IBusApiClient,BusApiClient>(client=>
        {
            client.BaseAddress=new Uri(EndpointSettings.BaseUrl);client.Timeout=TimeSpan.FromSeconds(15);
        });
        builder.Services.AddSingleton<SearchViewModel>();
        builder.Services.AddSingleton<SearchPage>();builder.Services.AddSingleton<DriverPage>();builder.Services.AddSingleton<AppShell>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
