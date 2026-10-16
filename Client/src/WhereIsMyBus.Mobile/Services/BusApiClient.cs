using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Core.Models;
namespace WhereIsMyBus.Mobile.Services;
public sealed class BusApiClient(HttpClient client) : IBusApiClient
{
    string BaseAPIUrl = "https://localhost:7081/";//new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).Build().GetSection("APIBaseURL").Value;
    GenericService service = new GenericService();
    private async Task<IReadOnlyList<T>> GetAsync<T>(string path,CancellationToken ct) => await client.GetFromJsonAsync<T[]>(path,ct) ?? [];
    private async Task<T> GetObjectAsync<T>(string path,CancellationToken ct) => await client.GetFromJsonAsync<T>(path,ct) ?? default(T);
    public Task<IReadOnlyList<BusModel>> SearchAsync(string registration,CancellationToken ct) => GetAsync<BusModel>("api/v1/buses/search?registrationNumber="+Uri.EscapeDataString(registration),ct);
    public Task<IReadOnlyList<BusModel>> ByRouteAsync(long fromId,long toId,CancellationToken ct) => GetAsync<BusModel>($"api/v1/buses/by-route?fromId={fromId}&toId={toId}",ct);
    public Task<IReadOnlyList<LocationModel>> LocationsAsync(CancellationToken ct) => GetAsync<LocationModel>("api/v1/locations/search",ct);

    public async Task<DriverDetails> GetDriverInformationAsync(DriverDetails driverDetails, CancellationToken ct = default)
    {
        string url = BaseAPIUrl + "api/drivers/getdriverinfo";
        var json = JsonConvert.SerializeObject(driverDetails);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await service.GetScalarObjectResultWithContent<DriverDetails>(url, content, ct);
    }

    public async Task<IReadOnlyList<string>> GetDriverBusRegistrations(string driver_mobile, CancellationToken ct = default)
    {
        string url = BaseAPIUrl + "api/drivers/busregistrations?driverMobile=" + driver_mobile; 
        return await service.GetScalarObjectResult<IReadOnlyList<string>>(url);
    }

    public async Task<IReadOnlyList<string>> GetDriverVillages(string driver_mobile, CancellationToken ct = default)
    {
        string url = BaseAPIUrl + "api/drivers/drivervillages?driverMobile=" + driver_mobile;
        return await service.GetScalarObjectResult<IReadOnlyList<string>>(url);
    }

}
