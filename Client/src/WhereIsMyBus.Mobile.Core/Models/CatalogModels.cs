namespace WhereIsMyBus.Mobile.Core.Models;
// API v1 transport models, maintained independently of the Server solution.
public sealed record BusModel(long Id,string RegistrationNumber,string OperatorName,string DisplayName,string TrackingStatus);
public sealed record LocationModel(long Id,string Name);
public sealed record RouteModel(long Id,string Name,long SourceId,long DestinationId,string SourceName,string DestinationName);
