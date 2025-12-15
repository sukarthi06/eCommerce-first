
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        var connStr = config.GetConnectionString("eCommerceConnection");

        services.AddDbContext<ProductDbContext>(options =>
            options.UseNpgsql(connStr));

        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connStr));

        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseNpgsql(connStr));
    })
    .Build();

using var scope = host.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

Console.WriteLine("Applying migrations...");

// Apply migrations for each DbContext
var productDb = serviceProvider.GetRequiredService<ProductDbContext>();
productDb.Database.Migrate();
Console.WriteLine("Product Db migrations applied.");

var orderDb = serviceProvider.GetRequiredService<OrderDbContext>();
orderDb.Database.Migrate();
Console.WriteLine("Order Db migrations applied.");

var authDb = serviceProvider.GetRequiredService<AuthenticationDbContext>();
authDb.Database.Migrate();
Console.WriteLine("Authentication Db migrations applied.");

// 👇 Seed default admin user
var existingUser = authDb.AppUsers.FirstOrDefault(u => u.Email == "admin@ecommerce.com");
if (existingUser != null)
{
    existingUser.Password = BCrypt.Net.BCrypt.HashPassword("string");
}
else
{
    existingUser = new AppUser
    {
        Name = "John Doe",
        TelephoneNumber = "1234567890",
        Address = "string",
        Email = "admin@ecommerce.com",
        Password = BCrypt.Net.BCrypt.HashPassword("string"), // You can hash this if needed
        Role = "Admin"
    };

    authDb.AppUsers.Add(existingUser);
}
authDb.SaveChanges();
Console.WriteLine("Seeded default admin user in Authentication Db.");

Console.WriteLine("All migrations applied successfully.");