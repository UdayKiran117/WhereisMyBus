using Microsoft.AspNetCore.Mvc;
using WhereIsMyBus.Application.Services;
using WhereIsMyBus.Contracts.Responses;
namespace WhereIsMyBus.API.Controllers;

[ApiController]
public sealed class CatalogController(CatalogService catalog) : ControllerBase
{
    [HttpGet("api/v1/locations/search")] public Task<IReadOnlyList<LocationResponse>> Locations([FromQuery] string? query, CancellationToken ct) => catalog.LocationsAsync(query, ct);
    [HttpGet("api/v1/routes")] public Task<IReadOnlyList<RouteResponse>> Routes(CancellationToken ct) => catalog.RoutesAsync(ct);
    [HttpGet("api/v1/routes/{id:long}")]
    public async Task<ActionResult<RouteResponse>> Route(long id, CancellationToken ct)
    {
        var route = await catalog.RouteAsync(id, ct); return route is null ? NotFound() : Ok(route);
    }
    [HttpGet("api/v1/app-info")] public AppInfoResponse Info() => new("Where Is My Bus", "v1", "1 Foundation", ["en", "te", "hi"], false);
}
