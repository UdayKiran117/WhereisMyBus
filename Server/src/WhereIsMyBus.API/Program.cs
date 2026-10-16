using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using WhereIsMyBus.API.Health;
using WhereIsMyBus.API.Middleware;
using WhereIsMyBus.Application.Extensions;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Infrastructure.Extensions;
using WhereIsMyBus.Infrastructure.Repositories;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false).AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IPasswordHasher<DriverDetails>, PasswordHasher<DriverDetails>>();
builder.Services.AddHealthChecks().AddCheck<MySqlHealthCheck>("mysql", tags: ["ready"]);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode=StatusCodes.Status429TooManyRequests;
    options.AddPolicy("catalog", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit=120,Window=TimeSpan.FromMinutes(1),QueueLimit=0,AutoReplenishment=true }));
});
var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
else { app.UseHsts(); app.UseHttpsRedirection(); }
app.UseRateLimiter();
app.MapHealthChecks("/health/live",new HealthCheckOptions { Predicate=_ => false });
app.MapHealthChecks("/health/ready",new HealthCheckOptions { Predicate=check => check.Tags.Contains("ready") });
app.MapControllers().RequireRateLimiting("catalog");
app.Run();
public partial class Program { }
