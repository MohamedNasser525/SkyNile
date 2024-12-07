using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SkyNile.Infrastructures;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails= new ProblemDetails(){
            Detail = $"API Error: {exception.Message}",
            Instance = "API",
            Status = (int) HttpStatusCode.InternalServerError,
            Title = "API Error",
            Type = "Server Error"
        };
        var response = JsonSerializer.Serialize(problemDetails);
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(response, cancellationToken);
        return true;
    }
}
