using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SkyNile.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    //private readonly ILogger _logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        //_logger.LogError(exception, exception.Message);
        var response = httpContext.Response;
        ProblemDetails details = new ProblemDetails()
        {
            Detail = $"API Error: {exception.Message}",
            Instance = "API",
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "API Error",
            Type = "Server Error"
        };
        var newResponse = JsonSerializer.Serialize(details);
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(newResponse, cancellationToken);
        return true;
    }
}
