using XchgRates.Server.Models;

namespace XchgRates.Server.Middlewares;

public class GlobalExceptionHandlingMiddleware(
    ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(ApiResponse.ForError(ex.Message).Value);
        }
    }
}
