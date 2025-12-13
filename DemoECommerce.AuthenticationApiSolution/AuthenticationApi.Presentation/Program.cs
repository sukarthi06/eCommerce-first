using AuthenticationApi.Infrastructure.DependencyInjection;
using AuthenticationApi.Presentation.Endpoints;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseInfrastructurePolicy();

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
        options.SwaggerEndpoint("/openapi/v1.json", "Auth API v1");

        // Make UI accessible at /swagger
        options.RoutePrefix = "swagger";
    });
    // This exposes swagger.json publicly
    //app.MapGet("/swagger/v1/swagger.json", () =>
    //    Results.File("swagger/v1/swagger.json", "application/json")
    //).AllowAnonymous();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapAuthenticationEndpoints();
app.Run();


//| URLs |
//| -------------------------- | ---------------------------- |
//| OpenAPI JSON(built -in) | **/openapi/v1.json * *         |
//| Swagger JSON(Swashbuckle) | **/swagger/v1/swagger.json * * |
//| Swagger UI | **/swagger * *                 |
//| Endpoint | **/api/authentication * *                |
