using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Core.ViewModels;
namespace WhereIsMyBus.Mobile.Views;
public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel viewModel;
    private readonly INearbyBusStationService _nearbyStations;
    private bool _findingStations;
    public SearchPage(SearchViewModel viewModel, INearbyBusStationService nearbyStations) { InitializeComponent(); BindingContext=this.viewModel=viewModel; _nearbyStations = nearbyStations; }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if(viewModel.Locations.Count==0) await viewModel.LoadLocationsCommand.ExecuteAsync(null);
    }
    protected override void OnDisappearing()
    {
        viewModel.LoadLocationsCommand.Cancel();viewModel.SearchCommand.Cancel();viewModel.SearchRouteCommand.Cancel();base.OnDisappearing();
    }
    private async void OnNearbyStationsClicked(object? sender, EventArgs e)
    {
        if (_findingStations)
            return;

        _findingStations = true;

        NearbyStationsButton.IsEnabled = false;
        NearbyStationsLoading.IsVisible = true;
        NearbyStationsLoading.IsRunning = true;

        NearbyStationsMessage.IsVisible = false;

        BindableLayout.SetItemsSource(NearbyStationsList, null);

        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(30));

        try
        {
            var permission = await Permissions.RequestAsync<
                Permissions.LocationWhenInUse>();

            if (permission != PermissionStatus.Granted)
            {
                ShowNearbyMessage(
                    "Allow location permission to find nearby stations.");
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(15)),
                timeout.Token);

            if (location is null)
            {
                ShowNearbyMessage(
                    "Unable to get your location. Please try again.");
                return;
            }

            var stations = await _nearbyStations.GetNearbyAsync(
                location.Latitude,
                location.Longitude,
                radiusKm: 5,
                ct: timeout.Token);

            var nearest = stations
                .Where(x =>
                    double.IsFinite(x.DistanceMeters) &&
                    x.DistanceMeters >= 0 &&
                    x.DistanceMeters <= 5000)
                .OrderBy(x => x.DistanceMeters)
                .Take(10)
                .ToList();

            BindableLayout.SetItemsSource(NearbyStationsList, nearest);

            ShowNearbyMessage(nearest.Count == 0
                ? "No bus stations found within 5 km."
                : "Nearest stations within 5 km — distances are straight-line.");
        }
        catch (FeatureNotEnabledException)
        {
            ShowNearbyMessage(
                "Turn on Location in your phone settings and try again.");
        }
        catch (PermissionException)
        {
            ShowNearbyMessage(
                "Location permission is required.");
        }
        catch (OperationCanceledException)
        {
            ShowNearbyMessage(
                "The request timed out. Please try again.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            ShowNearbyMessage(
                "Unable to load nearby stations. Check your connection.");
        }
        finally
        {
            _findingStations = false;

            NearbyStationsButton.IsEnabled = true;
            NearbyStationsLoading.IsVisible = false;
            NearbyStationsLoading.IsRunning = false;
        }
    }

    private void ShowNearbyMessage(string message)
    {
        NearbyStationsMessage.Text = message;
        NearbyStationsMessage.IsVisible = true;
    }
}
