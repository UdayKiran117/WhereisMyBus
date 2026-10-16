using Microsoft.Extensions.Options;
using MySqlConnector;
using WhereIsMyBus.Infrastructure.Configuration;
namespace WhereIsMyBus.Infrastructure.Persistence;
public sealed class MySqlConnectionFactory(IOptions<DatabaseOptions> options)
{
    public async Task<MySqlConnection> OpenAsync(CancellationToken ct)
    {
        var connection = new MySqlConnection(options.Value.ConnectionString);
        try { await connection.OpenAsync(ct); return connection; }
        catch { await connection.DisposeAsync(); throw; }
    }
}
