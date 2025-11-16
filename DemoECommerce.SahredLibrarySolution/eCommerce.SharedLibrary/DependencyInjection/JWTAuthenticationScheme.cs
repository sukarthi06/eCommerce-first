using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class JWTAuthenticationScheme
{
    public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
            {   
                options.RequireHttpsMetadata = false;                                         
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = config["Authentication:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = config["Authentication:Issuer"],
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero, // Optional: Adjust for clock skew if necessary
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:SecretKey"]!)),
                };
            });
        return services;
    }
}
