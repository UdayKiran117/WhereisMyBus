namespace WhereIsMyBus.Domain.Entities;
public sealed class Bus
{
    public long Id { get; init; }
    public string RegistrationNumber { get; init; } = string.Empty;
    public string OperatorName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
