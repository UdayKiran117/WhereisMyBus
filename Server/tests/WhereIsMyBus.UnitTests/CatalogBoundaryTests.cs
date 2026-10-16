using FluentAssertions;
using Moq;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Application.Services;
using WhereIsMyBus.Contracts.Responses;
using WhereIsMyBus.Domain.Entities;
namespace WhereIsMyBus.UnitTests;
public sealed class CatalogBoundaryTests
{
    [Theory][InlineData(1)][InlineData(100)]
    public async Task Valid_page_limits_preserve_bounded_query(int limit)
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        repo.Setup(r=>r.ListBusesAsync(limit,It.IsAny<CancellationToken>())).ReturnsAsync(new[]{new Bus { Id=5, RegistrationNumber="TS09CD5678",DisplayName="Express" }});
        var result=await new CatalogService(repo.Object).ListAsync(limit);
        result.Should().ContainSingle().Which.DisplayName.Should().Be("Express");repo.VerifyAll();
    }
    [Fact] public async Task Existing_bus_is_mapped_without_identity_data()
    {
        var repo=new Mock<ICatalogRepository>();repo.Setup(r=>r.GetBusAsync(5,It.IsAny<CancellationToken>())).ReturnsAsync(new Bus { Id=5,RegistrationNumber="TS09CD5678",OperatorName="Pilot" });
        var result=await new CatalogService(repo.Object).GetAsync(5);
        result!.OperatorName.Should().Be("Pilot");result.TrackingStatus.Should().Be("Unavailable");
    }
    [Fact] public async Task Missing_bus_returns_null()
    {
        var repo=new Mock<ICatalogRepository>();repo.Setup(r=>r.GetBusAsync(5,It.IsAny<CancellationToken>())).ReturnsAsync((Bus?)null);
        (await new CatalogService(repo.Object).GetAsync(5)).Should().BeNull();
    }
    [Theory][InlineData(0)][InlineData(-1)]
    public async Task Invalid_bus_id_does_not_access_repository(long id)
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        await FluentActions.Awaiting(()=>new CatalogService(repo.Object).GetAsync(id)).Should().ThrowAsync<ArgumentException>();repo.VerifyNoOtherCalls();
    }
    [Fact] public async Task Blank_location_query_returns_bounded_repository_choices()
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        repo.Setup(r=>r.SearchLocationsAsync("",It.IsAny<CancellationToken>())).ReturnsAsync(new[]{new LocationResponse(1,"Hyderabad")});
        (await new CatalogService(repo.Object).LocationsAsync(null)).Should().ContainSingle();repo.VerifyAll();
    }
    [Fact] public async Task Location_query_is_trimmed_and_cancellable()
    {
        using var cancellation=new CancellationTokenSource();var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        repo.Setup(r=>r.SearchLocationsAsync("Hyder",cancellation.Token)).ReturnsAsync(Array.Empty<LocationResponse>());
        await new CatalogService(repo.Object).LocationsAsync(" Hyder ",cancellation.Token);repo.VerifyAll();
    }
    [Fact] public async Task Oversized_location_query_does_not_access_repository()
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        await FluentActions.Awaiting(()=>new CatalogService(repo.Object).LocationsAsync(new string('x',81))).Should().ThrowAsync<ArgumentException>();repo.VerifyNoOtherCalls();
    }
    [Fact] public async Task Route_list_preserves_source_and_destination()
    {
        var repo=new Mock<ICatalogRepository>();repo.Setup(r=>r.ListRoutesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[]{new RouteResponse(1,"Pilot",1,3,"Hyderabad","Warangal")});
        var result=await new CatalogService(repo.Object).RoutesAsync();result.Should().ContainSingle().Which.DestinationName.Should().Be("Warangal");
    }
    [Fact] public async Task Missing_route_returns_null()
    {
        var repo=new Mock<ICatalogRepository>();repo.Setup(r=>r.GetRouteAsync(5,It.IsAny<CancellationToken>())).ReturnsAsync((RouteResponse?)null);
        (await new CatalogService(repo.Object).RouteAsync(5)).Should().BeNull();
    }
    [Fact] public async Task Invalid_route_id_is_rejected()
    {
        await FluentActions.Awaiting(()=>new CatalogService(Mock.Of<ICatalogRepository>()).RouteAsync(0)).Should().ThrowAsync<ArgumentException>();
    }
    [Fact] public async Task Repository_cancellation_is_not_replaced_with_empty_results()
    {
        var repo=new Mock<ICatalogRepository>();repo.Setup(r=>r.ListBusesAsync(50,It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());
        await FluentActions.Awaiting(()=>new CatalogService(repo.Object).ListAsync(50)).Should().ThrowAsync<OperationCanceledException>();
    }
}
