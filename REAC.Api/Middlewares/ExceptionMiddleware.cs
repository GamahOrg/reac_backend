using Core.Shared.Responses;
using System.Net;
using System.Text.Json;

namespace REAC.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            var errorMsg = ex.Message + "\n" + ex.InnerException?.Message + "\n" + ex.InnerException?.InnerException?.Message;
            //var errorMsg = _env.IsDevelopment() ? ex.StackTrace?.ToString() : ex.Message + ex.InnerException?.Message + ex.InnerException?.InnerException?.Message;
            ExceptionResponse response = new()
            {
                IsDone = false,
                Message = errorMsg + ex.ToString(),
                Exception = errorMsg + ex.ToString()
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
