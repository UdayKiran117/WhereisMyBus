using Microsoft.AspNetCore.Mvc;
using WhereIsMyBus.Application.Services;
using WhereIsMyBus.Contracts.Responses;
namespace WhereIsMyBus.API.Controllers;
[ApiController]
[Route("api/v1/buses")]
public sealed class BusesController(CatalogService catalog) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyList<BusResponse>> List([FromQuery] int limit = 50, CancellationToken ct = default) => catalog.ListAsync(limit, ct);
    [HttpGet("{id:long}")] public async Task<ActionResult<BusResponse>> Get(long id,CancellationToken ct)
    {
        var bus=await catalog.GetAsync(id,ct); return bus is null ? NotFound() : Ok(bus);
    }
    [HttpGet("search")] public Task<IReadOnlyList<BusResponse>> Search([FromQuery] string? registrationNumber,CancellationToken ct) => catalog.SearchAsync(registrationNumber,ct);
    [HttpGet("by-route")] public Task<IReadOnlyList<BusResponse>> ByRoute([FromQuery] long fromId,[FromQuery] long toId,CancellationToken ct) => catalog.ByRouteAsync(fromId,toId,ct);
}
