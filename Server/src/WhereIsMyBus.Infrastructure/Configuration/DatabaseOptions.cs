namespace WhereIsMyBus.Infrastructure.Configuration;
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; init; } = "";
    public int CommandTimeoutSeconds { get; init; } = 15;
}
