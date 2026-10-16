using FluentAssertions;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Application.Services;
using WhereIsMyBus.Contracts.Responses;
namespace WhereIsMyBus.ArchitectureTests;
public sealed class DependencyTests
{
    [Fact] public void Domain_has_no_application_infrastructure_or_api_reference()
    {
        typeof(Bus).Assembly.GetReferencedAssemblies().Select(a=>a.Name).Should().NotContain(n=>n!=null && (n.Contains("Application") || n.Contains("Infrastructure") || n.Contains("API")));
    }
    [Fact] public void Application_does_not_reference_infrastructure_or_api()
    {
        typeof(CatalogService).Assembly.GetReferencedAssemblies().Select(a=>a.Name).Should().NotContain(n=>n!=null && (n.Contains("Infrastructure") || n.Contains("WhereIsMyBus.API")));
    }
    [Fact] public void Contracts_do_not_reference_other_server_projects()
    {
        typeof(BusResponse).Assembly.GetReferencedAssemblies().Select(a=>a.Name).Should().NotContain(n=>n!=null && n.StartsWith("WhereIsMyBus."));
    }
}
