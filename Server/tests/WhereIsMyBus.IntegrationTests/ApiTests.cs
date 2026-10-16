using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Contracts.Responses;
using WhereIsMyBus.Domain.Entities;
namespace WhereIsMyBus.IntegrationTests;
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_,config)=>config.AddInMemoryCollection(new Dictionary<string,string?> { ["Database:ConnectionString"]="Server=127.0.0.1;Database=unused;User ID=unused;" }));
        builder.ConfigureServices(services=>
        {
            var repo=new Mock<ICatalogRepository>();
            repo.Setup(r=>r.SearchBusesAsync("TS09AB1234",It.IsAny<CancellationToken>())).ReturnsAsync(new Bus[] { new() { Id=1,RegistrationNumber="TS09AB1234",OperatorName="Pilot" } });
            repo.Setup(r=>r.SearchBusesAsync("TS09AB9999",It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Bus>());
            services.RemoveAll<ICatalogRepository>(); services.AddSingleton(repo.Object);
        });
    }
}
public sealed class ApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact] public async Task Live_health_does_not_require_database() => (await factory.CreateClient().GetAsync("/health/live")).StatusCode.Should().Be(HttpStatusCode.OK);
    [Fact] public async Task Public_search_returns_contract()
    {
        var result=await factory.CreateClient().GetFromJsonAsync<BusResponse[]>("/api/v1/buses/search?registrationNumber=ts09%20ab1234");
        result.Should().ContainSingle().Which.RegistrationNumber.Should().Be("TS09AB1234");
    }
    [Fact] public async Task Empty_search_returns_problem_details()
    {
        var response=await factory.CreateClient().GetAsync("/api/v1/buses/search");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }
    [Fact] public async Task Identical_route_returns_bad_request() => (await factory.CreateClient().GetAsync("/api/v1/buses/by-route?fromId=1&toId=1")).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    [Fact] public async Task Unknown_registration_returns_empty_array()
    {
        (await factory.CreateClient().GetFromJsonAsync<BusResponse[]>("/api/v1/buses/search?registrationNumber=TS09AB9999")).Should().BeEmpty();
    }
    [Fact] public async Task Swagger_is_available_in_development() => (await factory.CreateClient().GetAsync("/swagger/v1/swagger.json")).StatusCode.Should().Be(HttpStatusCode.OK);
    [Fact] public async Task App_info_truthfully_disables_driver_tracking()
    {
        (await factory.CreateClient().GetFromJsonAsync<AppInfoResponse>("/api/v1/app-info"))!.DriverTrackingEnabled.Should().BeFalse();
    }
}
