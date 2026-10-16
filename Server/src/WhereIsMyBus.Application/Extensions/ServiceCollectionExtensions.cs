using Microsoft.Extensions.DependencyInjection;
using WhereIsMyBus.Application.Services;
namespace WhereIsMyBus.Application.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services.AddScoped<CatalogService>();
}
