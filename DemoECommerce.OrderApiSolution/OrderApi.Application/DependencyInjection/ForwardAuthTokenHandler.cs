using Microsoft.AspNetCore.Http;

namespace OrderApi.Application.DependencyInjection
{
    public class ForwardAuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Authorization", token);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
