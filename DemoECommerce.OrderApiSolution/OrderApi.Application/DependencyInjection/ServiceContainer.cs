using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add application services here, e.g., AutoMapper, MediatR, etc.
        // Register http clients
        // Create dependency injections for application layer services
        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthTokenHandler>();
        services.AddHttpClient<IOrderService, Orderservice>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"] ?? string.Empty);
            client.Timeout = TimeSpan.FromSeconds(1);
        })
        .AddHttpMessageHandler<ForwardAuthTokenHandler>();

        // Create retry strategy using Polly
        var retyStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                // You can log retry attempts here if needed
                string msg = $"Retrying {args.AttemptNumber} time(s) due to Outcome: {args.Outcome}";
                LogException.LogToConsole(msg);
                LogException.LogToDebugger(msg);
                return ValueTask.CompletedTask;
            }
        };

        // Register resilience pipeline
        services.AddResiliencePipeline("my-retry-Pipeline", builder => { builder.AddRetry(retyStrategy); });

        return services;
    }
}
