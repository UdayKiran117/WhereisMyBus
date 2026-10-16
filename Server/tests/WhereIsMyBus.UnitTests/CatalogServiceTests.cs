using FluentAssertions;
using Moq;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Application.Services;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Domain.Validation;
namespace WhereIsMyBus.UnitTests;
public sealed class CatalogServiceTests
{
    [Theory][InlineData("ts09 ab1234","TS09AB1234")][InlineData(" TS-09-AB-1234 ","TS09AB1234")]
    public void Normalize_removes_spaces_and_hyphens(string input,string expected) => RegistrationNumber.Normalize(input).Should().Be(expected);
    [Theory][InlineData(null)][InlineData("")][InlineData("abc")][InlineData("TS09'1234")][InlineData("TS09💥1234")]
    public void Invalid_registration_is_rejected(string? input)
    {
        Action act=()=>RegistrationNumber.Normalize(input); act.Should().Throw<ArgumentException>();
    }
    [Fact] public async Task Search_passes_normalized_value_and_cancellation()
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        using var source=new CancellationTokenSource(); var ct=source.Token;
        repo.Setup(r=>r.SearchBusesAsync("TS09AB1234",ct)).ReturnsAsync(new Bus[] { new() { Id=1,RegistrationNumber="TS09AB1234",OperatorName="Pilot" } });
        var result=await new CatalogService(repo.Object).SearchAsync("ts09 ab1234",ct);
        result.Should().ContainSingle().Which.TrackingStatus.Should().Be("Unavailable");
        repo.VerifyAll();
    }
    [Fact] public async Task Unknown_bus_returns_empty_list()
    {
        var repo=new Mock<ICatalogRepository>(); repo.Setup(r=>r.SearchBusesAsync("TS09AB9999",It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Bus>());
        (await new CatalogService(repo.Object).SearchAsync("TS09AB9999")).Should().BeEmpty();
    }
    [Fact] public async Task Empty_search_does_not_query_database()
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict); var service=new CatalogService(repo.Object);
        await FluentActions.Awaiting(()=>service.SearchAsync(" ")).Should().ThrowAsync<ArgumentException>();
        repo.VerifyNoOtherCalls();
    }
    [Theory][InlineData(1,1)][InlineData(0,2)][InlineData(2,-1)]
    public async Task Invalid_route_is_rejected_without_database_access(long from,long to)
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        await FluentActions.Awaiting(()=>new CatalogService(repo.Object).ByRouteAsync(from,to)).Should().ThrowAsync<ArgumentException>();
        repo.VerifyNoOtherCalls();
    }
    [Fact] public async Task Route_search_preserves_direction()
    {
        var repo=new Mock<ICatalogRepository>(MockBehavior.Strict);
        repo.Setup(r=>r.GetBusesByRouteAsync(1,3,It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Bus>());
        await new CatalogService(repo.Object).ByRouteAsync(1,3); repo.VerifyAll();
    }
    [Theory][InlineData(0)][InlineData(101)] public async Task Invalid_limit_is_rejected(int limit)
    {
        await FluentActions.Awaiting(()=>new CatalogService(Mock.Of<ICatalogRepository>()).ListAsync(limit)).Should().ThrowAsync<ArgumentException>();
    }
}
