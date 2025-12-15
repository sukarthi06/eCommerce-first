
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>
        (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            var connectionString = config.GetConnectionString("eCommerceConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'eCommerceConnection' not found.");
            }
            options.UseNpgsql(connectionString, npgsqlOptionsAction =>
            {
                npgsqlOptionsAction.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
            // For SQL Server, uncomment the following lines
            //options.UseSqlServer(connectionString, sqlOptions =>
            //{
            //    //sqlOptions.MigrationsAssembly(typeof(TContext).Assembly.FullName);
            //    sqlOptions.EnableRetryOnFailure(
            //        maxRetryCount: 5,
            //        maxRetryDelay: TimeSpan.FromSeconds(30),
            //        errorNumbersToAdd: null);
            //});
        });
        // Add other shared services here as needed
        
        // Configure Serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
            restrictedToMinimumLevel:Serilog.Events.LogEventLevel.Information, 
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day)
            .CreateLogger();

        //Add JWT Authentication Scheme
        JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        //var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
        //if (env != null && !env.IsDevelopment())
        //{
        //    //app.UseHsts();
            
        //}
        app.UseMiddleware<Middleware.ListenOnlyToApiGateway>();
        app.UseMiddleware<Middleware.GlobalException>();
        
        return app;
    }
}
