using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Core.Models;
namespace WhereIsMyBus.Mobile.Core.ViewModels;
public partial class SearchViewModel(IBusApiClient api,IConnectivityService connectivity,ITextService text) : ObservableObject
{
    [ObservableProperty] private string registrationNumber="";
    [ObservableProperty] private LocationModel? fromLocation;
    [ObservableProperty] private LocationModel? toLocation;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string message="";
    public ObservableCollection<BusModel> Buses { get; }=[];
    public ObservableCollection<LocationModel> Locations { get; }=[];
    public string Title=>text["FindBus"];
    public string Subtitle=>text["Subtitle"];
    public string NumberLabel=>text["BusNumber"];
    public string SearchLabel=>text["Search"];
    public string FromLabel=>text["From"];
    public string ToLabel=>text["To"];
    public string RouteLabel=>text["FindRoute"];
    public string RetryLabel=>text["Retry"];
    public string TrackingLabel=>text["Unavailable"];
    public string Hint=>text["StartHint"];

    public event Action<BusModel>? BusSelected;

    [RelayCommand]
    private void OpenBus(BusModel? bus)
    {
        if (bus is null || IsBusy)
            return;

        BusSelected?.Invoke(bus);
    }
    [RelayCommand] private void ChangeLanguage(string language)
    {
        text.SetLanguage(language); Message="";
        foreach(var property in new[]{nameof(Title),nameof(Subtitle),nameof(NumberLabel),nameof(SearchLabel),nameof(FromLabel),nameof(ToLabel),nameof(RouteLabel),nameof(RetryLabel),nameof(TrackingLabel),nameof(Hint)}) OnPropertyChanged(property);
    }
    [RelayCommand] private async Task LoadLocationsAsync(CancellationToken ct)
    {
        if(IsBusy) return;
        await ExecuteAsync(async()=>
        {
            var items=await api.LocationsAsync(ct);
            Locations.Clear(); foreach(var item in items) Locations.Add(item);
        },ct);
    }
    [RelayCommand] private async Task SearchAsync(CancellationToken ct)
    {
        if(IsBusy) return;
        var normalized=RegistrationNumber.Replace(" ","").Replace("-","").ToUpperInvariant();
        if(RegistrationNumber.Length>30 || !Regex.IsMatch(normalized,"^[A-Z0-9]{6,15}$")) { Buses.Clear();Message=text["EnterNumber"];return; }
        await ExecuteAsync(async()=>Show(await api.SearchAsync(normalized,ct)),ct);
    }

    [RelayCommand] private async Task SearchRouteAsync(CancellationToken ct)
    {
        if(IsBusy) return;
        if(FromLocation is null || ToLocation is null || FromLocation.Id==ToLocation.Id) { Buses.Clear();Message=text["SelectRoute"];return; }
        var from=FromLocation.Id;var to=ToLocation.Id;
        await ExecuteAsync(async()=>Show(await api.ByRouteAsync(from,to,ct)),ct);
    }
    private void Show(IReadOnlyList<BusModel> items)
    {
        Buses.Clear();

        foreach (var item in items)
            Buses.Add(item);

        Message = items.Count == 0 ? text["Empty"] : "";

        if (items.Count == 1)
            BusSelected?.Invoke(items[0]);
    }
    private async Task ExecuteAsync(Func<Task> action,CancellationToken ct)
    {
        if(!connectivity.IsConnected) { Buses.Clear();Message=text["Offline"];return; }
        IsBusy=true;Message=text["Loading"];
        try { await action(); if(Message==text["Loading"]) Message=""; }
        catch(OperationCanceledException) when(ct.IsCancellationRequested) { Message=""; }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        { Buses.Clear();Message=text["Error"]; }
        finally { IsBusy=false; }
    }
}
