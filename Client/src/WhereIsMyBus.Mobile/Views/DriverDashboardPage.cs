using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Services;

namespace WhereIsMyBus.Mobile.Views;

public partial class DriverDashboardPage : ContentPage
{
    private readonly IDriverDashboardService _service;

    private bool _busy;
    private bool _loaded;
    private bool _checkedIn;
    BusApiClient _apiservices;
    public DriverDashboardPage(IDriverDashboardService service,HttpClient client, string driverName, string driverMobile)
    {
        InitializeComponent();

        _service = service;
        _apiservices = new BusApiClient(client);
        DriverNameLabel.Text = driverName;
        DriverMobileLabel.Text = driverMobile;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //uncomment after
        if (!_loaded)
            await LoadBusesAsync();
    }

    private async Task LoadBusesAsync()
    {
        if (_busy || _checkedIn)
            return;

        SetBusy(true);
        StatusLabel.IsVisible = false;

        try
        {
            BusPicker.SelectedIndex = -1;
            BusPicker.ItemsSource = null;

            DestinationPicker.SelectedIndex = -1;
            DestinationPicker.ItemsSource = null;

            //var buses = await _service.GetAssignedBusesAsync();
            var buses = await _apiservices.GetDriverBusRegistrations(DriverMobileLabel.Text);
            var villages = await _apiservices.GetDriverVillages(DriverMobileLabel.Text);
            //BusPicker.ItemsSource = buses.ToList();
            //_loaded = true;

            if (buses == null || buses.Count == 0)
            {
                SetMessage("No bus is assigned to you. Please contact your agent.",
                    true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            SetMessage("Unable to load buses. Check your connection and retry.",
                true);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnBusChanged(object? sender, EventArgs e)
    {
        if (_busy || _checkedIn)
            return;

        DestinationPicker.SelectedIndex = -1;
        DestinationPicker.ItemsSource = null;

        if (BusPicker.SelectedItem is not DashboardBus bus)
        {
            SetBusy(false);
            return;
        }

        SetBusy(true);
        StatusLabel.IsVisible = false;

        try
        {
            var destinations =
                await _service.GetDestinationsAsync(bus.Id);

            DestinationPicker.ItemsSource = destinations.ToList();

            if (destinations.Count == 0)
            {
                SetMessage("No destinations are configured for this bus.", true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            SetMessage("Unable to load destinations. Select the bus again to retry.", true);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnCheckInClicked(object? sender, EventArgs e)
    {
        if (_busy || _checkedIn)
            return;

        if (BusPicker.SelectedItem is not DashboardBus bus)
        {
            SetMessage("Please select your bus.", true);
            BusPicker.Focus();
            return;
        }

        if (DestinationPicker.SelectedItem
            is not DashboardDestination destination)
        {
            SetMessage("Please select your destination.", true);
            DestinationPicker.Focus();
            return;
        }

        SetBusy(true);

        try
        {
            var result = await _service.CheckInAsync(new DriverCheckInRequest(bus.Id, destination.Id));

            if (!result.Success)
            {
                SetMessage(result.Message, true);
                return;
            }

            _checkedIn = true;
            CheckInButton.Text = "Checked In";

            SetMessage(
                $"Checked in successfully.\n" +
                $"Bus: {bus.RegistrationNumber}\n" +
                $"Destination: {destination.Name}",
                false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            SetMessage(
                "Check-in could not be confirmed. Check your trip status before retrying.",
                true);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnReloadClicked(object? sender, EventArgs e)
    {
        await LoadBusesAsync();
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;

        BusyIndicator.IsRunning = busy;
        BusyIndicator.IsVisible = busy;

        BusPicker.IsEnabled = !busy && !_checkedIn;
        RetryButton.IsEnabled = !busy && !_checkedIn;

        var hasDestinations =
            DestinationPicker.ItemsSource?.Count > 0;

        DestinationPicker.IsEnabled =
            !busy && !_checkedIn && hasDestinations;

        CheckInButton.IsEnabled =
            !busy &&
            !_checkedIn &&
            BusPicker.SelectedItem is DashboardBus &&
            hasDestinations;
    }

    private void SetMessage(string message, bool isError)
    {
        StatusLabel.Text = message;
        StatusLabel.TextColor = Color.FromArgb(isError ? "#C62828" : "#087F5B");

        StatusLabel.IsVisible = true;
    }
}