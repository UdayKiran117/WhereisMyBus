using Microsoft.Extensions.Diagnostics.HealthChecks;
using WhereIsMyBus.Infrastructure.Persistence;
namespace WhereIsMyBus.API.Health;
public sealed class MySqlHealthCheck(MySqlConnectionFactory factory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try { await using var db = await factory.OpenAsync(cancellationToken); await db.PingAsync(cancellationToken); return HealthCheckResult.Healthy(); }
        catch (Exception ex) when (ex is not OperationCanceledException) { return HealthCheckResult.Unhealthy("Database unavailable."); }
    }
}
