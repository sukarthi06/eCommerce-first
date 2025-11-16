namespace ApiGateway.Presentation.Middlewares
{
    public class AttachSignatureToRequest(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers["eComm-Api-Gateway"] = "Signed";
            await next(context);
        }
    }
}
