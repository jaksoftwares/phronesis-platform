using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Phronesis.Domain.Common.Exceptions;
using Phronesis.Shared.Responses;
using System.Net;
using System.Text.Json;

namespace Phronesis.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DomainException domainEx => (StatusCode: (int)HttpStatusCode.BadRequest, Message: domainEx.Message),
            _ => (StatusCode: (int)HttpStatusCode.InternalServerError, Message: "An unexpected error occurred.")
        };

        context.Response.StatusCode = response.StatusCode;

        var apiResponse = ApiResponse.Failure(response.Message);
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(apiResponse, options);

        await context.Response.WriteAsync(json);
    }
}
