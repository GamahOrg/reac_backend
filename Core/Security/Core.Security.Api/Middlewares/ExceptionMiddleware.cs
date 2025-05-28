using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Instance;
using Core.Log.Application.Manager;
using Core.Log.Domain.Shared.Enums;
using Core.Shared.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace Core.Security.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;
    private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider
    private readonly FileLogger _fileLogger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        IHostEnvironment env,
        IServiceProvider serviceProvider,
        FileLogger fileLogger)
    {
        _next = next;
        _env = env;
        _serviceProvider = serviceProvider; // Assign IServiceProvider
        _fileLogger = fileLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception using FileLogger
        await _fileLogger.Log(new LogDto { Message = exception.ToString() });

        // Return a generic error response
        var errorMsg = exception.Message + "\n" + exception.InnerException?.Message + "\n" + exception.InnerException?.InnerException?.Message;

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        ExceptionResponse response = new()
        {
            IsDone = false,
            Message = errorMsg + exception.ToString(),
            Exception = errorMsg + exception.ToString()
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        IHostEnvironment env,
        FileLogger fileLogger)
    {
        _next = next;
        _env = env;
        _fileLogger = fileLogger; // Assign the singleton FileLogger
    }
}
