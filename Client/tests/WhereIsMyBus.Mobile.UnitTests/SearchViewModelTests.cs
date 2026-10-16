using FluentAssertions;
using Moq;
using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Core.Models;
using WhereIsMyBus.Mobile.Core.Services;
using WhereIsMyBus.Mobile.Core.ViewModels;
namespace WhereIsMyBus.Mobile.UnitTests;
public sealed class SearchViewModelTests
{
    private sealed class Preferences : IUserPreferences { public string Language { get; set; }="en"; }
    private static SearchViewModel Create(IBusApiClient api,bool online=true) => new(api,Mock.Of<IConnectivityService>(c=>c.IsConnected==online),new TextService(new Preferences()));
    [Fact] public async Task Empty_search_shows_validation_and_does_not_call_api()
    {
        var api=new Mock<IBusApiClient>(MockBehavior.Strict);var vm=Create(api.Object);
        await vm.SearchCommand.ExecuteAsync(null);vm.Message.Should().Contain("valid bus");api.VerifyNoOtherCalls();
    }
    [Fact] public async Task Valid_search_normalizes_number()
    {
        var api=new Mock<IBusApiClient>(MockBehavior.Strict);
        api.Setup(a=>a.SearchAsync("TS09AB1234",It.IsAny<CancellationToken>())).ReturnsAsync(new[]{new BusModel(1,"TS09AB1234","Pilot","Bus","Unavailable")});
        var vm=Create(api.Object);vm.RegistrationNumber="ts09 ab1234";
        await vm.SearchCommand.ExecuteAsync(null);vm.Buses.Should().ContainSingle();vm.IsBusy.Should().BeFalse();api.VerifyAll();
    }
    [Fact] public async Task No_results_displays_empty_message()
    {
        var api=new Mock<IBusApiClient>();api.Setup(a=>a.SearchAsync(It.IsAny<string>(),It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<BusModel>());
        var vm=Create(api.Object);vm.RegistrationNumber="TS09AB9999";await vm.SearchCommand.ExecuteAsync(null);vm.Message.Should().Contain("No buses");
    }
    [Fact] public async Task Offline_search_does_not_call_api()
    {
        var api=new Mock<IBusApiClient>(MockBehavior.Strict);var vm=Create(api.Object,false);vm.RegistrationNumber="TS09AB1234";
        await vm.SearchCommand.ExecuteAsync(null);vm.Message.Should().Contain("offline");api.VerifyNoOtherCalls();
    }
    [Fact] public async Task Timeout_returns_error_and_clears_busy_state()
    {
        var api=new Mock<IBusApiClient>();api.Setup(a=>a.SearchAsync(It.IsAny<string>(),It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException());
        var vm=Create(api.Object);vm.RegistrationNumber="TS09AB1234";await vm.SearchCommand.ExecuteAsync(null);
        vm.Message.Should().Contain("try again");vm.IsBusy.Should().BeFalse();
    }
    [Fact] public async Task Route_search_sends_selected_ids()
    {
        var api=new Mock<IBusApiClient>(MockBehavior.Strict);api.Setup(a=>a.ByRouteAsync(1,3,It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<BusModel>());
        var vm=Create(api.Object);vm.FromLocation=new(1,"Hyderabad");vm.ToLocation=new(3,"Warangal");await vm.SearchRouteCommand.ExecuteAsync(null);api.VerifyAll();
    }
    [Fact] public async Task Same_route_location_does_not_call_api()
    {
        var api=new Mock<IBusApiClient>(MockBehavior.Strict);var vm=Create(api.Object);vm.FromLocation=new(1,"A");vm.ToLocation=new(1,"A");
        await vm.SearchRouteCommand.ExecuteAsync(null);vm.Message.Should().Contain("different");api.VerifyNoOtherCalls();
    }
    [Fact] public void Changing_language_updates_labels_and_persists_choice()
    {
        var preferences=new Preferences();var text=new TextService(preferences);var vm=new SearchViewModel(Mock.Of<IBusApiClient>(),Mock.Of<IConnectivityService>(),text);
        vm.ChangeLanguageCommand.Execute("te");vm.Title.Should().Be("మీ బస్సును కనుగొనండి");preferences.Language.Should().Be("te");
        vm.ChangeLanguageCommand.Execute("hi");vm.SearchLabel.Should().Be("बस खोजें");
    }
    [Fact] public void Invalid_saved_language_falls_back_to_English() => new TextService(new Preferences { Language="xx" }).Language.Should().Be("en");
    [Fact] public async Task Load_locations_populates_selectors()
    {
        var api=new Mock<IBusApiClient>();api.Setup(a=>a.LocationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[]{new LocationModel(1,"Hyderabad")});
        var vm=Create(api.Object);await vm.LoadLocationsCommand.ExecuteAsync(null);vm.Locations.Should().ContainSingle();
    }
    [Fact] public async Task Concurrent_search_is_prevented()
    {
        var completion=new TaskCompletionSource<IReadOnlyList<BusModel>>();var api=new Mock<IBusApiClient>();
        api.Setup(a=>a.SearchAsync(It.IsAny<string>(),It.IsAny<CancellationToken>())).Returns(completion.Task);
        var vm=Create(api.Object);vm.RegistrationNumber="TS09AB1234";var first=vm.SearchCommand.ExecuteAsync(null);
        await vm.SearchCommand.ExecuteAsync(null);completion.SetResult(Array.Empty<BusModel>());await first;
        api.Verify(a=>a.SearchAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()),Times.Once);
    }
}
