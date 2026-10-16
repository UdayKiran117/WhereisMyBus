using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Infrastructure.Configuration;
using WhereIsMyBus.Infrastructure.Persistence;
using WhereIsMyBus.Infrastructure.Repositories;
namespace WhereIsMyBus.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Database:ConnectionString is required.")
            .Validate(o => o.CommandTimeoutSeconds is >= 1 and <= 120, "Command timeout must be 1 to 120 seconds.")
            .ValidateOnStart();
        services.AddSingleton<MySqlConnectionFactory>();
        services.AddScoped<ICatalogRepository,CatalogRepository>();
        return services;
    }
}
