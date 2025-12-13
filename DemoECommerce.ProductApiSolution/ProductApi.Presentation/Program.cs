using ProductApi.Infrastructure.DependencyInjection;
using ProductApi.Presentation.Endpoints;

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

app.UseInfrastructurePolicies();

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
        options.SwaggerEndpoint("/openapi/v1.json", "Products API v1");

        // Make UI accessible at /swagger
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapProductEndpoints();

app.Run();