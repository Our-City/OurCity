using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.Utils;

namespace OurCity.Api.Middlewares;

//TODO: tenant provider is in DB..

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITenantProvider tenantProvider)
    {
        tenantProvider.TenantName = context.Request.Host.Host.Split('.')[0];

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenant(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
