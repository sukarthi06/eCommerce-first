using OrderApi.Application.DependencyInjection;
using OrderApi.Infrastructure.DependencyInjection;
using OrderApi.Presentation.Endpoints;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseInfrastructurePolicy();
app.MapOrderServicesEndpoints();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Enable Swagger JSON (Swashbuckle)
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    // Enable Swagger UI
    app.UseSwaggerUI(options =>
    {
        // Load .NET 9 native openapi document
        options.SwaggerEndpoint("/openapi/v1.json", "Order API v1");

        // Make UI accessible at /swagger
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHttpsRedirection();
}

app.Run();

//| URLs |
//| -------------------------- | ---------------------------- |
//| OpenAPI JSON(built -in) | **/openapi/v1.json * *         |
//| Swagger JSON(Swashbuckle) | **/swagger/v1/swagger.json * * |
//| Swagger UI | **/swagger * *                 |
//| Endpoint | **/api/authentication * * 