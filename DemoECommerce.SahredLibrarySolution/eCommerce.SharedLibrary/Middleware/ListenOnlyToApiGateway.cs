
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenOnlyToApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // -----------------------------------------------------------------
        // 🚀 1. Exclude Swagger paths so Ocelot can fetch swagger.json
        // -----------------------------------------------------------------
        if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        // -----------------------------------------------------------------
        // 🚀 2. Also exclude OpenAPI / OpenApi.json (Minimal API support)
        // -----------------------------------------------------------------
        if (path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("swagger", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("eComm-Api-Gateway", out var apiGatewayHeader) || apiGatewayHeader != "Signed")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied. Requests must come through the API Gateway.");
            return;
        }
        await next(context);
    }
}
