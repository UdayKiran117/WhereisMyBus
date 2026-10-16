using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
namespace WhereIsMyBus.API.Middleware;
public sealed class ApiExceptionHandler(IProblemDetailsService problems, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var status = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            MySqlException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
        if (status>=500) logger.LogError("Request {TraceId} failed with {ExceptionType}",context.TraceIdentifier,exception.GetType().Name);
        var detail = status==400 ? exception.Message : "The service is temporarily unavailable. Please try again.";
        context.Response.StatusCode=status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext=context,
            ProblemDetails=new ProblemDetails { Status=status,Title=status==400 ? "Invalid request" : "Service error",Detail=detail,Extensions={ ["traceId"]=context.TraceIdentifier } }
        });
    }
}
