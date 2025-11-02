using Serilog.Context;

namespace OurCity.Api.Middlewares;

/// <summary>
/// Associates a correlation id with every request
/// </summary>
/// <credits>
/// Code taken largely from ChatGPT, asking for middleware that adds correlation id to http requests + adds it to serilog logger
/// </credits>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? context.TraceIdentifier;

        context.Request.Headers["X-Correlation-ID"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
