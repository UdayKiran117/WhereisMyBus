namespace WhereIsMyBus.Contracts.Responses;
public sealed record BusResponse(long Id, string RegistrationNumber, string OperatorName, string DisplayName, string TrackingStatus = "Unavailable");
public sealed record LocationResponse(long Id, string Name);
public sealed record RouteResponse(long Id, string Name, long SourceId, long DestinationId, string SourceName, string DestinationName);
public sealed record AppInfoResponse(string Name, string ApiVersion, string Milestone, string[] Languages, bool DriverTrackingEnabled);
