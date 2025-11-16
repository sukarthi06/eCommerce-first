
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenOnlyToApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("eComm-Api-Gateway", out var apiGatewayHeader) || apiGatewayHeader != "Signed")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied. Requests must come through the API Gateway.");
            return;
        }
        await next(context);
    }
}
